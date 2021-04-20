using CqrsExample.Domain.BaseAbstractions.Commands;
using CqrsExample.Domain.Features.Shopping.CreateList;
using CqrsExample.Domain.Features.Shopping.UpdateList;
using Microsoft.Extensions.DependencyInjection;

namespace CqrsExample.Api.Configurations
{
    public static class CommandsConfiguration
    {
        public static IServiceCollection AddCommandHandlers(this IServiceCollection services) =>
            services.AddSingleton<ICommandHandler<CreateShoppingListCommand, CreateShoppingListResult>, CreateShoppingListHandler>()
                    .AddSingleton<ICommandHandler<UpdateShoppingListCommand, UpdateShoppingListResult>, UpdateShoppingListHandler>();
    }
}