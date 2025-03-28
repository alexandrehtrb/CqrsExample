using System;
using System.Collections.Generic;

namespace CqrsExample.Domain.Features.Shopping.Common.Entities;

public sealed record ShoppingListItem(
    int Quantity,
    string ItemName);

public sealed class ShoppingList
{
    public Guid Id { get; set; }
    public string Title { get; set; }
    public ShoppingListItem[] Items { get; set; }

    // Empty constructor for JSON deserialization
#nullable disable warnings
    public ShoppingList()
    {
    }
#nullable restore warnings

    public ShoppingList(string title)
    {
        Id = Guid.NewGuid();
        Title = title;
        Items = Array.Empty<ShoppingListItem>();
    }

    public void Update(string title, ShoppingListItem[] items)
    {
        Title = title;
        Items = items;
    }

    // We need this to be class instead of record
    // and to override Equals, because default record
    // equality does not deeply compare sequences / lists
    public override bool Equals(object? obj) =>
        obj is ShoppingList list &&
        Id.Equals(list.Id) &&
        Title == list.Title &&
        EqualityComparer<ShoppingListItem[]>.Default.Equals(Items, list.Items);

    public override int GetHashCode() =>
        HashCode.Combine(Id, Title, Items);
}