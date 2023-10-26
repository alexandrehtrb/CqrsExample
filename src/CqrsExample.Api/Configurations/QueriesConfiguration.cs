using CqrsExample.Domain.BaseAbstractions.Queries;
using CqrsExample.Domain.Features.Shopping.GetList;
using Microsoft.Extensions.DependencyInjection;

namespace CqrsExample.Api.Configurations;

public static class QueriesConfiguration
{
    public static IServiceCollection AddQueryHandlers(this IServiceCollection services) =>
        services.AddSingleton<IQueryHandler<GetShoppingListQuery, GetShoppingListResult>, GetShoppingListHandler>();
}