using System;
using System.Threading.Tasks;

namespace CqrsExample.Domain.Features.Shopping.Repositories
{
    public interface IShoppingListReadRepository
    {
        Task<A?> QueryAsync<A>(Guid id) where A : class, new();
    }
}