using System.Text.Json;
using System.Text.Json.Serialization;
using Microsoft.Extensions.DependencyInjection;

namespace CqrsExample.Api.Configurations
{
    public static class JsonConfiguration
    {
        public static readonly JsonSerializerOptions DefaultJsonOptions = SetupDefaultJsonOptions(new JsonSerializerOptions());

        public static IMvcBuilder AddDefaultJsonOptions(this IMvcBuilder mvcBuilder) =>
            mvcBuilder.AddJsonOptions(o => SetupDefaultJsonOptions(o.JsonSerializerOptions));

        private static JsonSerializerOptions SetupDefaultJsonOptions(JsonSerializerOptions options)
        {
            options.Converters.Add(new JsonStringEnumConverter());
            options.WriteIndented = false;
            options.IgnoreNullValues = true;
            return options;
        }
    }
}