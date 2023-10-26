using System;
using System.Collections.Generic;
using System.Reflection;
using System.Threading.Tasks;
using CqrsExample.Domain.Features.Shopping.Entities;
using CqrsExample.Domain.Features.Shopping.Repositories;

namespace CqrsExample.Infrastructure.Features.Shopping.Repositories;

// Em um aplicação de uso real, a classe concreta do repositório se conectaria a um banco de dados, a uma API, ou a outras fontes de dados.
// Aqui está sendo usado um banco de dados em memória por ser mais simples, apenas para demonstrar arquitetura

// In a real-world application, the repository's concrete class  would connect to a database, an API, or some other data source
// Here we are using an in-memory database for being simpler, only to demonstrate architecture
public sealed class ShoppingListInMemoryRepository : IShoppingListReadRepository, IShoppingListWriteRepository
{
    private readonly IDictionary<Guid, ShoppingList> db;

    public ShoppingListInMemoryRepository() =>
        this.db = new Dictionary<Guid, ShoppingList>();

    public Task<A?> QueryAsync<A>(Guid id) where A : class, new()
    {
        // Este método lê do item apenas as propriedades requeridas na classe <A>
        // A idéia é obter apenas as informações que você quer retornar, sem precisar obter o item inteiro

        // This query method only reads from the item the properties required in the <A> class
        // The idea is to only query the information you wish to return, not the whole item

        if (this.db.TryGetValue(id, out var value))
        {
            var item = value;
            var instance = Activator.CreateInstance<A>();
            foreach (var destinationPropInfo in typeof(A).GetProperties())
            {
                var sourcePropInfo = typeof(ShoppingList).GetProperty(destinationPropInfo.Name);
                if (sourcePropInfo != null)
                {
                    destinationPropInfo.SetValue(instance, sourcePropInfo.GetValue(item));
                }
            }
            return Task.FromResult((A?)instance);
        }
        else
        {
            return Task.FromResult((A?)null);
        }
    }

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
            this.db.Add(shoppingList.Id, shoppingList);
            return Task.FromResult(true);
        }
    }

    public Task<bool> UpdateAsync(ShoppingList shoppingList)
    {
        if (this.db.ContainsKey(shoppingList.Id))
        {
            this.db.Remove(shoppingList.Id);
            this.db.Add(shoppingList.Id, shoppingList);
            return Task.FromResult(true);
        }
        else
        {
            return Task.FromResult(false);
        }
    }
}