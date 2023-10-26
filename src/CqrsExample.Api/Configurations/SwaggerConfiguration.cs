using System;
using System.Collections.Generic;
using System.IO;
using System.Reflection;
using Microsoft.AspNetCore.Builder;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.OpenApi.Models;
using Swashbuckle.AspNetCore.Filters;

namespace CqrsExample.Api.Configurations;

public static class SwaggerConfiguration
{
    public static IServiceCollection AddSwaggerConfiguration(this IServiceCollection services)
    {
        services.AddSwaggerGen(options =>
        {
            options.SwaggerDoc(
                "v1",
                new OpenApiInfo()
                {
                    Title = "Shopping List API",
                    Version = "v1",
                    Description = "A shopping list API using CQRS architecture."
                });

            string commentFileName = $"{Assembly.GetExecutingAssembly().GetName().Name}.xml";
            string commentFilePath = Path.Combine(AppContext.BaseDirectory, commentFileName);
            options.IncludeXmlComments(commentFilePath);
            options.ExampleFilters();
        });
        services.AddSwaggerExamplesFromAssemblies(Assembly.GetEntryAssembly());
        return services;
    }

    public static IApplicationBuilder UseSwaggerConfiguration(this IApplicationBuilder app)
    {
        app.UseSwagger(c =>
            {
                c.RouteTemplate = "swagger/{documentName}/swagger.json";
                c.PreSerializeFilters.Add((swaggerDoc, httpReq) =>
                    swaggerDoc.Servers = new List<OpenApiServer>()
                    {
                        new OpenApiServer()
                        {
                            Url = $"{httpReq.Scheme}://{httpReq.Host.Value}/"
                        }
                    });
            })
            .UseSwaggerUI(c => c.SwaggerEndpoint("v1/swagger.json", "v1"));

        return app;
    }
}