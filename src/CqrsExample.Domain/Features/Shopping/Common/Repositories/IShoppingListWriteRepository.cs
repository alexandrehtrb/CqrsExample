using System;
using System.Threading.Tasks;
using CqrsExample.Domain.Features.Shopping.Common.Entities;

namespace CqrsExample.Domain.Features.Shopping.Common.Repositories;

public interface IShoppingListWriteRepository
{
    Task<ShoppingList?> GetAsync(Guid id);
    Task<bool> InsertAsync(ShoppingList shoppingList);
    Task<bool> UpdateAsync(ShoppingList shoppingList);
}