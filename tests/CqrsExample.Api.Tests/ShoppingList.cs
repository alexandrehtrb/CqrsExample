namespace CqrsExample.Api.Tests;

public sealed record ShoppingListItem(int Quantity, string ItemName);

public sealed record ShoppingList(Guid Id, string Title, List<ShoppingListItem> Items);