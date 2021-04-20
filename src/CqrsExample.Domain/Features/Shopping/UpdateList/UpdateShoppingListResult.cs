using System;
using CqrsExample.Domain.BaseAbstractions.Commands;

namespace CqrsExample.Domain.Features.Shopping.UpdateList
{
    public sealed class UpdateShoppingListResult : CommandResult
    {
        public override bool Equals(object? obj) =>
            obj is UpdateShoppingListResult result;

        public override int GetHashCode() =>
            HashCode.Combine(this);
    }
}