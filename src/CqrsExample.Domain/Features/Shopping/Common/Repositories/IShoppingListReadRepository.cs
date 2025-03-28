using CqrsExample.Domain.Features.Shopping.Common.Entities;

namespace CqrsExample.Domain.Features.Shopping.Common.Repositories;

public interface IShoppingListReadRepository
{
    // In this example, we only have a Get method,
    // but we could add specific database and cache queries here.

    Task<ShoppingList?> GetAsync(Guid id);
}