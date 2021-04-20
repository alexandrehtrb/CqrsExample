using System;
using System.Collections.Generic;
using CqrsExample.Domain.BaseAbstractions.Queries;
using CqrsExample.Domain.Features.Shopping.Entities;

namespace CqrsExample.Domain.Features.Shopping.GetList
{
    public sealed class GetShoppingListResult : QueryResult
    {
        #nullable disable warnings
        public Guid Id { get; set; }
        public string Title { get; set; }
        public ShoppingListItem[] Items { get; set; }
        #nullable restore warnings
        
        public override bool Equals(object? obj) =>
            obj is GetShoppingListResult result &&
            Id.Equals(result.Id) &&
            Title == result.Title &&
            EqualityComparer<ShoppingListItem[]>.Default.Equals(Items, result.Items);

        public override int GetHashCode() =>
            HashCode.Combine(Id, Title, Items);
    }
}