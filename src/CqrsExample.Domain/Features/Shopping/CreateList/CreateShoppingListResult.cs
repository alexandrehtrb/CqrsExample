using System;
using System.Collections.Generic;
using CqrsExample.Domain.BaseAbstractions.Commands;
using CqrsExample.Domain.Features.Shopping.Common.Entities;

namespace CqrsExample.Domain.Features.Shopping.CreateList;

public sealed class CreateShoppingListResult : CommandResult
{
#nullable disable warnings
    public Guid Id { get; set; }
    public string Title { get; set; }
    public ShoppingListItem[] Items { get; set; }
#nullable restore warnings

    public CreateShoppingListResult(ShoppingList shoppingList)
    {
        Id = shoppingList.Id;
        Title = shoppingList.Title;
        Items = Array.Empty<ShoppingListItem>();
    }

    public override bool Equals(object? obj) =>
        obj is CreateShoppingListResult result &&
        Id.Equals(result.Id) &&
        Title == result.Title &&
        EqualityComparer<ShoppingListItem[]>.Default.Equals(Items, result.Items);

    public override int GetHashCode() =>
        HashCode.Combine(Id, Title, Items);
}