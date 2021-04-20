using System;
using System.Collections.Generic;
using System.Linq;
using CqrsExample.Domain.BaseAbstractions.Errors;
using CqrsExample.Domain.Features.Shopping.Entities;

namespace CqrsExample.Domain.Features.Shopping.UpdateList
{
    public static class UpdateShoppingListCommandValidator
    {
        public static bool Validate(this UpdateShoppingListCommand cmd, out Error[]? errors)
        {
            var errorsList = new List<Error>();

            ValidateId(cmd, errorsList);
            ValidateTitle(cmd, errorsList);
            ValidateItemsNotNull(cmd, errorsList);
            ValidateItemsNames(cmd, errorsList);
            ValidateItemsQuantities(cmd, errorsList);
            ValidateItemsNotRepeated(cmd, errorsList);

            errors = errorsList.Count > 0 ? errorsList.ToArray() : null;
            return errorsList.Count == 0;
        }

        private static void ValidateId(UpdateShoppingListCommand cmd, IList<Error> errorsList)
        {
            if (cmd.GetId() == null || cmd.GetId() == Guid.Empty)
                errorsList.Add(new Error(ShoppingListErrors.InvalidId));
        }

        private static void ValidateTitle(UpdateShoppingListCommand cmd, IList<Error> errorsList)
        {
            if (string.IsNullOrWhiteSpace(cmd.Title))
                errorsList.Add(new Error(ShoppingListErrors.BlankTitle));
        }

        private static void ValidateItemsNotNull(UpdateShoppingListCommand cmd, IList<Error> errorsList)
        {
            if (cmd.Items == null)
                errorsList.Add(new Error(ShoppingListErrors.ItemsNull));
        }

        private static void ValidateItemsNames(UpdateShoppingListCommand cmd, IList<Error> errorsList)
        {
            if (cmd.Items != null && cmd.Items.Any(i => string.IsNullOrWhiteSpace(i.ItemName)))
                errorsList.Add(new Error(ShoppingListErrors.BlankItemName));
        }

        private static void ValidateItemsQuantities(UpdateShoppingListCommand cmd, IList<Error> errorsList)
        {
            if (cmd.Items != null)
            {
                foreach (ShoppingListItem i in cmd.Items)
                {
                    if (i.Quantity <= 0)
                        errorsList.Add(new Error(ShoppingListErrors.ItemQuantityZeroOrLess, i.ItemName));
                }
            }
        }

        private static void ValidateItemsNotRepeated(UpdateShoppingListCommand cmd, IList<Error> errorsList)
        {
            if (cmd.Items != null)
            {
                var hasRepeatedItem = cmd.Items.GroupBy(i => i.ItemName, i => i)
                                               .Select(d => (d.Key, d.Count()))
                                               .Any(d => d.Item2 > 1);
                if (hasRepeatedItem)
                    errorsList.Add(new Error(ShoppingListErrors.RepeatedItems));
            }
        }
    }
}