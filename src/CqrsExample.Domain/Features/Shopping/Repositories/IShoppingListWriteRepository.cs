using System;
using System.Threading.Tasks;
using CqrsExample.Domain.Features.Shopping.Entities;

namespace CqrsExample.Domain.Features.Shopping.Repositories
{
    public interface IShoppingListWriteRepository
    {
        Task<ShoppingList?> GetAsync(Guid id);
        Task<bool> InsertAsync(ShoppingList shoppingList);
        Task<bool> UpdateAsync(ShoppingList shoppingList);
    }
}