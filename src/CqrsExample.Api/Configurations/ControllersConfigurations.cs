using CqrsExample.Api.Configurations.Filters;
using Microsoft.Extensions.DependencyInjection;

namespace CqrsExample.Api.Configurations
{
    public static class ControllersConfigurations
    {
        public static IMvcBuilder AddCustomControllers(this IServiceCollection services) =>
            services.AddControllers(c =>
                {
                    c.Filters.Add<LogActionFilter>();
                    c.Conventions.Add(new FeatureFolderConvention());
                });
    }
}