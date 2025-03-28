using System.Text.Encodings.Web;
using System.Text.Json;
using System.Text.Json.Serialization;
using CqrsExample.Domain.Features.Shopping.CreateList;
using CqrsExample.Domain.Features.Shopping.GetList;
using CqrsExample.Domain.Features.Shopping.UpdateList;
using Microsoft.AspNetCore.Http.Json;

namespace CqrsExample.Api.Configurations;

public static class JsonConfiguration
{
    public static readonly JsonSerializerOptions DefaultJsonOptions =
        SetupDefaultJsonOptions(new());

    public static IServiceCollection ConfigureJsonOptions(this IServiceCollection services) =>
        services.Configure<JsonOptions>(o => SetupDefaultJsonOptions(o.SerializerOptions));

    public static JsonSerializerOptions SetupDefaultJsonOptions(JsonSerializerOptions options)
    {
        options.Encoder = JavaScriptEncoder.UnsafeRelaxedJsonEscaping;
        options.WriteIndented = false;
        options.DefaultIgnoreCondition = JsonIgnoreCondition.WhenWritingNull;
        options.PropertyNamingPolicy = JsonNamingPolicy.CamelCase;
        options.AllowTrailingCommas = true;
        options.ReadCommentHandling = JsonCommentHandling.Skip;
        options.TypeInfoResolver = ApiJsonSrcGenContext.Default;
        return options;
    }
}

[JsonSerializable(typeof(GetShoppingListQuery))]
[JsonSerializable(typeof(CreateShoppingListCommand))]
[JsonSerializable(typeof(UpdateShoppingListCommand))]
[JsonSerializable(typeof(GetShoppingListResult))]
[JsonSerializable(typeof(CreateShoppingListResult))]
[JsonSerializable(typeof(UpdateShoppingListResult))]
[JsonSerializable(typeof(Microsoft.AspNetCore.Mvc.ProblemDetails))]
public partial class ApiJsonSrcGenContext : JsonSerializerContext
{
}