using System;
using System.Collections.Generic;

namespace CqrsExample.Domain.Features.Shopping.Entities
{
    public sealed class ShoppingListItem
    {
        public int Quantity { get; set; }
        public string ItemName { get; set; }

        // Empty constructor for JSON deserialization
        #nullable disable warnings
        public ShoppingListItem()
        {
        }
        #nullable restore warnings

        public ShoppingListItem(int quantity, string itemName)
        {
            Quantity = quantity;
            ItemName = itemName;
        }

        public override bool Equals(object? obj) =>
            obj is ShoppingListItem item &&
            Quantity == item.Quantity &&
            ItemName == item.ItemName;

        public override int GetHashCode() =>
            HashCode.Combine(Quantity, ItemName);
    }

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
            Items = new ShoppingListItem[0];
        }

        public void Update(string title, ShoppingListItem[] items)
        {
            this.Title = title;
            this.Items = items;
        }

        public override bool Equals(object? obj) =>
            obj is ShoppingList list &&
            Id.Equals(list.Id) &&
            Title == list.Title &&
            EqualityComparer<ShoppingListItem[]>.Default.Equals(Items, list.Items);

        public override int GetHashCode() =>
            HashCode.Combine(Id, Title, Items);
    }
}