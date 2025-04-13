#if NATIVEAOT

using CqrsExample.Api.Configurations;
using System.Text.Json;

namespace CqrsExample.Api.Logging;

public sealed class ClefLoggerProvider : ILoggerProvider
{
    public ILogger CreateLogger(string categoryName) =>
        new ClefLogger(categoryName, new LoggerExternalScopeProvider());

    public void Dispose() { }
}

// This is a very opinionated Logger that
// outputs CLEF <https://clef-json.org/> messages to Console.
// Works with NativeAOT and trimming, no external libraries required,
// and includes scopes in messages.

public sealed class ClefLogger(
    string categoryName,
    IExternalScopeProvider? scopeProvider) : ILogger
{
    private static readonly string[] logsToIgnore = [
        "Request starting",
        "Executing",
        "Setting HTTP status code",
        "Executed",
        "Writing value",
        "Write content",
        "Sending file",
        "Request reached the end of the middleware pipeline without being handled by application code."
    ];

    public IDisposable? BeginScope<TState>(TState state)
        where TState : notnull =>
        scopeProvider?.Push(state) ?? default;

    public bool IsEnabled(LogLevel logLevel) => true;

    private static bool FilterProperties(KeyValuePair<string, object?> kv) =>
        kv.Key != "{OriginalFormat}"
        && kv.Value != null
        && (kv.Value is string s ? !string.IsNullOrEmpty(s) : true);

    private static bool ShouldIgnoreLog(string? originalFormat) =>
        originalFormat is null || logsToIgnore.Any(originalFormat.StartsWith);

    public void Log<TState>(LogLevel logLevel, EventId eventId, TState state, Exception? exception, Func<TState, Exception?, string> formatter)
    {
        if (state is not IEnumerable<KeyValuePair<string, object?>> msgProps)
            return;

        string? originalFormat = (string?)msgProps.FirstOrDefault(kv => kv.Key == "{OriginalFormat}").Value;

        if (ShouldIgnoreLog(originalFormat))
            return;

        List<IEnumerable<KeyValuePair<string, object?>>>? scopes = new();
        scopeProvider?.ForEachScope((scope, st) =>
        {
            if (scope is IEnumerable<KeyValuePair<string, object?>> scopeItems)
            {
                scopes.Add(scopeItems.Where(FilterProperties));
            }
        }, state);

        IEnumerable<KeyValuePair<string, object?>> standardProps =
        [
            new("@i", eventId.Name),
            new("@t", DateTime.UtcNow.ToString("yyyy-MM-ddTHH:mm:ss.fffffffZ")),
            new("@c", categoryName),
            new("@l", Enum.GetName(logLevel)),
            //new("@m", formatter(state, exception)),
            new("@mt", originalFormat),
            new("@x", exception?.ToString())
        ];

        var msg = standardProps
            .Concat(msgProps.Where(FilterProperties))
            .Concat(scopes.SelectMany(x => x))
            // avoid repeated keys
            .DistinctBy(kv => kv.Key)
            // removing null values for JSON
            .Where(x => x.Value is not null)
            .ToDictionary();

        try
        {
            WriteLogLine(msg);
        }
        catch (Exception jsonEx)
        {
            msg = standardProps
                .Append(new("WARNING", "Could not serialize original logging message to CLEF JSON."))
                .Append(new("JsonException", jsonEx.ToString()))
                .ToDictionary();

            WriteLogLine(msg);
        }
    }

    // TODO: Use queue to avoid concurrency
    // Change output if you want: file, remote log server, etc.
    private void WriteLogLine(Dictionary<string, object?> msg)
    {
        string json = JsonSerializer.Serialize(msg, ApiJsonSrcGenContext.Default.DictionaryStringObject);
        Console.OutputEncoding = System.Text.Encoding.UTF8;
        Console.WriteLine(json);
    }
}

#endif