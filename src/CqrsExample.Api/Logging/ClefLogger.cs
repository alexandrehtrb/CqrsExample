#if NATIVEAOT

using CqrsExample.Api.Configurations;
using System.Text.Json.Serialization;
using System.Text.Json;
using System.Collections.Concurrent;

namespace CqrsExample.Api.Logging;

// This is a very opinionated Logger that
// outputs CLEF <https://clef-json.org/> messages to Console.
// Works with NativeAOT and trimming, no external libraries required,
// and includes scopes in messages.

public sealed class ClefLogger(
    string categoryName,
    IExternalScopeProvider? scopeProvider) : ILogger
{
    private static readonly EventId requestFinishedEventId = new(0x49848, "HttpRequestFinished");

    public IDisposable? BeginScope<TState>(TState state) where TState : notnull =>
        scopeProvider?.Push(state) ?? default;

    public bool IsEnabled(LogLevel logLevel) =>
        true;

    public void Log<TState>(LogLevel logLevel, EventId eventId, TState state, Exception? exception, Func<TState, Exception?, string> formatter)
    {
        static bool IsNotUnwantedProperty(KeyValuePair<string, object?> kv) =>
            kv.Key != "{OriginalFormat}" && (kv.Value is string s ? !string.IsNullOrEmpty(s) : kv.Value != null);

        if (state is not IEnumerable<KeyValuePair<string, object?>> dict)
        {
            return;
        }

        string? originalFormat = (string?)dict.FirstOrDefault(kv => kv.Key == "{OriginalFormat}").Value;

        string[] msgsToIgnore = [
            "Request starting",
            "Executing",
            "Setting HTTP status code",
            "Executed",
            "Writing value",
            "Write content",
            "Sending file",
        ];
        foreach (string msgStart in msgsToIgnore)
        {
            if (originalFormat is not null && originalFormat.StartsWith(msgStart))
                return;
        }

        if (originalFormat is not null && originalFormat.StartsWith("Request finished"))
        {
            eventId = requestFinishedEventId;
        }

        List<Dictionary<string, object?>> scopesList = new();

        scopeProvider?.ForEachScope((scope, st) =>
        {
            if (scope is IEnumerable<KeyValuePair<string, object?>> scopeItems)
            {
                scopesList.Add(scopeItems.Where(IsNotUnwantedProperty).ToDictionary(kv => kv.Key, kv => kv.Value));
            }
            else
            {
                scopesList.Add(new() { { "Scope", scope?.ToString() } });
            }
        }, state);

        var filteredProps = dict.Where(IsNotUnwantedProperty);

        ClefLogMessage msg = new(
            EventId: eventId.Name,
            Timestamp: DateTimeOffset.Now,
            CategoryName: categoryName,
            LogLevel: Enum.GetName(logLevel),
            Message: formatter(state, exception),
            Exception: exception?.ToString(),
            Properties: filteredProps.Any() ? filteredProps.ToDictionary(kv => kv.Key, kv => kv.Value) : null,
            Scopes: scopesList.Count > 0 ? scopesList : null);

        try
        {
            // TODO: Use queue and buffer to avoid concurrency
            Console.WriteLine(JsonSerializer.Serialize(msg, JsonConfiguration.JsonCtx.ClefLogMessage));
        }
        catch (Exception jsonEx)
        {
            msg = new(
                EventId: null,
                Timestamp: DateTimeOffset.Now,
                CategoryName: categoryName,
                LogLevel: Enum.GetName(LogLevel.Warning),
                Message: formatter(state, exception),
                Exception: exception?.ToString(),
                Properties: new()
                {
                    { "WARNING", "Could not serialize original logging message to CLEF JSON." },
                    { "ClefJsonException", jsonEx.ToString() }
                },
                Scopes: null);

            // TODO: Use queue and buffer to avoid concurrency
            Console.WriteLine(JsonSerializer.Serialize(msg, JsonConfiguration.JsonCtx.ClefLogMessage));
        }
        finally
        {
            Console.WriteLine();
        }
    }
}

public sealed class ClefLoggerProvider : ILoggerProvider
{
    public ILogger CreateLogger(string categoryName) => new ClefLogger(categoryName, new LoggerExternalScopeProvider());
    public void Dispose() { }
}

public sealed record ClefLogMessage(
    [property: JsonPropertyName("@i")] string? EventId,
    [property: JsonPropertyName("@t")] DateTimeOffset Timestamp,
    [property: JsonPropertyName("@c")] string CategoryName,
    [property: JsonPropertyName("@l")] string? LogLevel,
    [property: JsonPropertyName("@m")] string? Message,
    [property: JsonPropertyName("@x")] string? Exception,
    [property: JsonPropertyName("@props")] Dictionary<string, object?>? Properties,
    [property: JsonPropertyName("@scopes")] List<Dictionary<string, object?>>? Scopes);

#endif