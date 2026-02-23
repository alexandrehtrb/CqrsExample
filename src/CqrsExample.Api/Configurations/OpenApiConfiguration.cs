#if !NATIVEAOT

using System.Net;
using System.Text.Json;
using System.Text.Json.Serialization.Metadata;
using Microsoft.AspNetCore.OpenApi;
using Microsoft.AspNetCore.Mvc;
using Microsoft.OpenApi;
using CqrsExample.Domain.BaseAbstractions;
using System.Text.Json.Nodes;

namespace CqrsExample.Api.Configurations;

public static class OpenApiConfiguration
{
    public static IServiceCollection AddOpenApiConfiguration(this IServiceCollection services)
    {
        services.AddOpenApi("v1", options =>
        {
            options.OpenApiVersion = Microsoft.OpenApi.OpenApiSpecVersion.OpenApi3_1;

            options.AddOperationTransformer((op, ctx, ct) => Endpoints.DescribeGetShoppingListExamples(op));
            options.AddOperationTransformer((op, ctx, ct) => Endpoints.DescribeCreateShoppingListExamples(op));
            options.AddOperationTransformer((op, ctx, ct) => Endpoints.DescribeUpdateShoppingListExamples(op));

            options.AddDocumentTransformer((document, context, cancellationToken) =>
            {
                document.Info = new OpenApiInfo()
                {
                    Version = "v1",
                    Title = "Shopping List API",
                    Description = "A shopping list API using CQRS architecture.",
                    Contact = new OpenApiContact
                    {
                        Name = "Contoso Support",
                        Email = "support@contoso.com"
                    }
                };
                return Task.CompletedTask;
            });

            options.AddSchemaTransformer((schema, context, cancellationToken) =>
            {
                if (context.JsonTypeInfo.Type == typeof(decimal))
                {
                    // default schema for decimal is just type: number.  Add format: decimal
                    schema.Format = "decimal";
                }
                return Task.CompletedTask;
            });

            // Always inline enum schemas
            options.CreateSchemaReferenceId = (type) => type.Type.IsEnum ? null : OpenApiOptions.CreateDefaultSchemaReferenceId(type);
        });

        return services;
    }

    public static void AddRequestExample<TRequest>(this OpenApiOperation op, TRequest reqExample, JsonTypeInfo jsonTypeInfo)
    {
        string json = JsonSerializer.Serialize(reqExample, jsonTypeInfo);
        op.RequestBody ??= new OpenApiRequestBody();
        ((OpenApiRequestBody)op.RequestBody).Content ??= new Dictionary<string, OpenApiMediaType>();
        op.RequestBody!.Content![Endpoints.ContentTypeJson].Examples = new Dictionary<String, IOpenApiExample>()
        {
            { "reqExample", new OpenApiExample() { Value = JsonNode.Parse(json) } }
        };
    }

    private static OpenApiResponse SetupOpenApiResponseContent(this OpenApiOperation op, HttpStatusCode httpStatusCode)
    {
        string statusCode = ((int)httpStatusCode).ToString();
        op.Responses ??= new();
        IOpenApiResponse? oaiResponse;
        if (!op.Responses.TryGetValue(statusCode, out oaiResponse))
        {
            oaiResponse = new OpenApiResponse();
            op.Responses.Add(statusCode, oaiResponse);
        }
        ((OpenApiResponse)oaiResponse).Content ??= new Dictionary<string, OpenApiMediaType>();
        return ((OpenApiResponse)oaiResponse);
    }

    public static void AddResponseExample<TResponse>(this OpenApiOperation op, HttpStatusCode httpStatusCode, TResponse resExample, JsonTypeInfo jsonTypeInfo)
    {
        var oaiResponse = op.SetupOpenApiResponseContent(httpStatusCode);
        oaiResponse.Content![Endpoints.ContentTypeJson].Example = JsonNode.Parse(JsonSerializer.Serialize(resExample, jsonTypeInfo));
    }

    public static void AddResponseExamples<TResponse>(this OpenApiOperation op, HttpStatusCode httpStatusCode, Dictionary<string, TResponse> responseExamples, JsonTypeInfo jsonTypeInfo)
    {
        var oaiResponse = op.SetupOpenApiResponseContent(httpStatusCode);
        oaiResponse.Content![Endpoints.ContentTypeJson].Examples =
            responseExamples.ToDictionary(
                kv => Guid.NewGuid().ToString(),
                kv => (IOpenApiExample)(new OpenApiExample()
                {
                    Summary = kv.Key,
                    Value = JsonNode.Parse(JsonSerializer.Serialize(kv.Value, jsonTypeInfo))
                }));
    }

    public static void AddProblemDetailsResponseExamples(this OpenApiOperation op, HttpStatusCode httpStatusCode, Dictionary<string, (string, string)> responseExamples)
    {
        var oaiResponse = op.SetupOpenApiResponseContent(httpStatusCode);
        oaiResponse.Content![Endpoints.ContentTypeProblemDetailsJson].Examples =
            responseExamples.ToDictionary(
                kv => Guid.NewGuid().ToString(),
                kv => (IOpenApiExample)(new OpenApiExample()
                {
                    Summary = kv.Key,
                    Value = JsonNode.Parse(JsonSerializer.Serialize(
                        new ProblemDetails()
                        {
                            Status = (int)httpStatusCode,
                            Type = MapHttpStatusCodeToProblemDetailsType(httpStatusCode),
                            Title = kv.Value.Item1,
                            Detail = kv.Value.Item2
                        },
                        ApiJsonSrcGenContext.Default.ProblemDetails))
                }));
    }

    private static string MapHttpStatusCodeToProblemDetailsType(HttpStatusCode httpStatusCode) =>
        Enum.GetName(httpStatusCode switch
        {
            HttpStatusCode.Unauthorized => CommandResultFailureType.Unauthorized,
            HttpStatusCode.Forbidden => CommandResultFailureType.Forbidden,
            HttpStatusCode.BadRequest => CommandResultFailureType.Validation,
            HttpStatusCode.NotFound => CommandResultFailureType.NotFound,
            HttpStatusCode.UnprocessableEntity => CommandResultFailureType.Unprocessable,
            _ => CommandResultFailureType.InternalError
        })!;

}

#endif