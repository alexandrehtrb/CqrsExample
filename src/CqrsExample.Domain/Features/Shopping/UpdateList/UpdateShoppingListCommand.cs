using System;
using CqrsExample.Domain.BaseAbstractions.Commands;
using CqrsExample.Domain.Features.Shopping.Common.Entities;

namespace CqrsExample.Domain.Features.Shopping.UpdateList;

public sealed class UpdateShoppingListCommand : Command
{
    private Guid? Id;
    public string? Title { get; set; }
    public ShoppingListItem[]? Items { get; set; }

    public void SetId(Guid? id) => this.Id = id;

    public Guid? GetId() => this.Id;
}