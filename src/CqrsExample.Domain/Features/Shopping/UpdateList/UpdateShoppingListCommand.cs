using System;
using CqrsExample.Domain.BaseAbstractions.Commands;
using CqrsExample.Domain.Features.Shopping.Entities;

namespace CqrsExample.Domain.Features.Shopping.UpdateList
{
    public sealed class UpdateShoppingListCommand : Command
    {
        private Guid? Id;
        public string? Title { get; set; }
        public ShoppingListItem[]? Items { get; set; }

        public void SetId(Guid? id) => Id = id;

        public Guid? GetId() => Id;
    }
}