using System.Text.Encodings.Web;
using System.Text.Json;
using System.Text.Json.Serialization;
using System.Text.Unicode;
using CqrsExample.Domain.Features.Shopping.CreateList;
using CqrsExample.Domain.Features.Shopping.GetList;
using CqrsExample.Domain.Features.Shopping.UpdateList;
using Microsoft.AspNetCore.Http.Json;

namespace CqrsExample.Api.Configurations;

public static class JsonConfiguration
{
    public static IServiceCollection ConfigureJsonOptions(this IServiceCollection services) =>
        services.Configure<JsonOptions>(o => o.SerializerOptions.TypeInfoResolver = ApiJsonSrcGenContext.Default);
}

[JsonSerializable(typeof(GetShoppingListQuery))]
[JsonSerializable(typeof(CreateShoppingListCommand))]
[JsonSerializable(typeof(UpdateShoppingListCommand))]
[JsonSerializable(typeof(GetShoppingListResult))]
[JsonSerializable(typeof(CreateShoppingListResult))]
[JsonSerializable(typeof(UpdateShoppingListResult))]
[JsonSerializable(typeof(Microsoft.AspNetCore.Mvc.ProblemDetails))]
#if NATIVEAOT
// needed for ClefLogger
[JsonSerializable(typeof(Dictionary<string, object?>))]
[JsonSerializable(typeof(double))]
[JsonSerializable(typeof(long))]
#endif
[JsonSourceGenerationOptions(
    WriteIndented = false,
    PropertyNamingPolicy = JsonKnownNamingPolicy.CamelCase,
    DefaultIgnoreCondition = JsonIgnoreCondition.WhenWritingNull,
    UseStringEnumConverter = true,
    AllowTrailingCommas = true,
    ReadCommentHandling = JsonCommentHandling.Skip,
    GenerationMode = JsonSourceGenerationMode.Default)]
public partial class ApiJsonSrcGenContext : JsonSerializerContext
{
    static ApiJsonSrcGenContext()
    {
        // replace default context
        Default = new ApiJsonSrcGenContext(CreateJsonSerializerOptions(Default));
    }

    private static JsonSerializerOptions CreateJsonSerializerOptions(ApiJsonSrcGenContext defaultContext)
    {
        var encoderSettings = new TextEncoderSettings();
        encoderSettings.AllowRange(UnicodeRanges.All);
        //encoderSettings.AllowRange(UnicodeRanges.BasicLatin);
        //encoderSettings.AllowRange(UnicodeRanges.Latin1Supplement); // æøå etc.

        var options = new JsonSerializerOptions(defaultContext.GeneratedSerializerOptions!)
        {
            Encoder = JavaScriptEncoder.Create(encoderSettings)
        };

        return options;
    }
}