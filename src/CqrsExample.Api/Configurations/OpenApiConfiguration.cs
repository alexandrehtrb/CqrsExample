using Microsoft.AspNetCore.OpenApi;
using Microsoft.OpenApi.Models;

namespace CqrsExample.Api.Configurations;

public static class OpenApiConfiguration
{
    public static IServiceCollection AddOpenApiConfiguration(this IServiceCollection services)
    {
        services.AddOpenApi("v1", options =>
        {
            options.OpenApiVersion = Microsoft.OpenApi.OpenApiSpecVersion.OpenApi3_0;

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
}