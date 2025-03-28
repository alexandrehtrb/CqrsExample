using CqrsExample.Domain.Features.Shopping.Common.Repositories;
using CqrsExample.Domain.Features.Shopping.CreateList;
using CqrsExample.Domain.Features.Shopping.GetList;
using CqrsExample.Domain.Features.Shopping.UpdateList;
using CqrsExample.Infrastructure.Features.Shopping.Repositories;

namespace CqrsExample.Api;

public static class DI
{
    public static IServiceCollection AddDependencies(this IServiceCollection services, IConfiguration config)
    {
        var repo = new ShoppingListInMemoryRepository();
        services.AddSingleton<IShoppingListReadRepository>(repo);
        services.AddSingleton<IShoppingListWriteRepository>(repo);

        services.AddSingleton<CreateShoppingListHandler>();
        services.AddSingleton<GetShoppingListHandler>();
        services.AddSingleton<UpdateShoppingListHandler>();

        return services;
    }
}