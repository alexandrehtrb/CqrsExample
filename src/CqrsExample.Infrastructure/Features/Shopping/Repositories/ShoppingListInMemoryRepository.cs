using System.Collections.Concurrent;
using CqrsExample.Domain.Features.Shopping.Common.Entities;
using CqrsExample.Domain.Features.Shopping.Common.Repositories;

namespace CqrsExample.Infrastructure.Features.Shopping.Repositories;

// In a real-world application, the repository's concrete class
// would connect to a database, an API, or some other data source.
// 
// Here we are using an in-memory database for being simpler,
// only to demonstrate architecture.

public sealed class ShoppingListInMemoryRepository : IShoppingListReadRepository, IShoppingListWriteRepository
{
    private readonly ConcurrentDictionary<Guid, ShoppingList> db;

    public ShoppingListInMemoryRepository() =>
        this.db = new();

    public Task<ShoppingList?> GetAsync(Guid id)
    {
        if (this.db.TryGetValue(id, out var value))
            return Task.FromResult((ShoppingList?)value);
        else
            return Task.FromResult((ShoppingList?)null);
    }

    public Task<bool> InsertAsync(ShoppingList shoppingList)
    {
        if (this.db.ContainsKey(shoppingList.Id))
        {
            return Task.FromResult(false);
        }
        else
        {
            this.db.TryAdd(shoppingList.Id, shoppingList);
            return Task.FromResult(true);
        }
    }

    public Task<bool> UpdateAsync(ShoppingList shoppingList)
    {
        if (this.db.ContainsKey(shoppingList.Id))
        {
            this.db.TryRemove(shoppingList.Id, out _);
            this.db.TryAdd(shoppingList.Id, shoppingList);
            return Task.FromResult(true);
        }
        else
        {
            return Task.FromResult(false);
        }
    }
}