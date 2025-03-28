using CqrsExample.Domain.Features.Shopping.Common.Entities;

namespace CqrsExample.Domain.Features.Shopping.Common.Repositories;

public interface IShoppingListWriteRepository
{
    // Here we add operations that perform changes plus
    // a method for retrieving an item from the database.
    // 
    // This sometimes coincides with other methods
    // from the read repo interfaces.

    Task<ShoppingList?> GetAsync(Guid id);
    Task<bool> InsertAsync(ShoppingList shoppingList);
    Task<bool> UpdateAsync(ShoppingList shoppingList);
}