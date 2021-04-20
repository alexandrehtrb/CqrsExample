using System;
using CqrsExample.Domain.BaseAbstractions.Queries;

namespace CqrsExample.Domain.Features.Shopping.GetList
{
    public sealed class GetShoppingListQuery : Query
    {
        public readonly Guid? Id;

        public GetShoppingListQuery(Guid? id) => this.Id = id;
    }
}