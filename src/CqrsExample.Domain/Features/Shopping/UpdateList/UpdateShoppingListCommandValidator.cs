using CqrsExample.Domain.BaseAbstractions;

namespace CqrsExample.Domain.Features.Shopping.UpdateList;

public static class UpdateShoppingListCommandValidator
{
    public static bool Validate(this UpdateShoppingListCommand cmd, out CqrsError? error) =>
        ValidateId(cmd, out error) &&
        ValidateTitle(cmd, out error) &&
        ValidateItemsNotNull(cmd, out error) &&
        ValidateItemsNames(cmd, out error) &&
        ValidateItemsQuantities(cmd, out error) &&
        ValidateItemsNotRepeated(cmd, out error);

    private static bool ValidateId(UpdateShoppingListCommand cmd, out CqrsError? error)
    {
        if (cmd.Id == null || cmd.Id == Guid.Empty)
        {
            error = new(ShoppingListErrors.InvalidId);
            return false;
        }
        else
        {
            error = null;
            return true;
        }
    }

    private static bool ValidateTitle(UpdateShoppingListCommand cmd, out CqrsError? error)
    {
        if (string.IsNullOrWhiteSpace(cmd.Title))
        {
            error = new(ShoppingListErrors.BlankTitle);
            return false;
        }
        else
        {
            error = null;
            return true;
        }
    }

    private static bool ValidateItemsNotNull(UpdateShoppingListCommand cmd, out CqrsError? error)
    {
        if (cmd.Items == null)
        {
            error = new(ShoppingListErrors.ItemsNull);
            return false;
        }
        else
        {
            error = null;
            return true;
        }
    }

    private static bool ValidateItemsNames(UpdateShoppingListCommand cmd, out CqrsError? error)
    {
        if (cmd.Items != null && cmd.Items.Any(i => string.IsNullOrWhiteSpace(i.ItemName)))
        {
            error = new(ShoppingListErrors.BlankItemName);
            return false;
        }
        else
        {
            error = null;
            return true;
        }
    }

    private static bool ValidateItemsQuantities(UpdateShoppingListCommand cmd, out CqrsError? error)
    {
        if (cmd.Items != null)
        {
            foreach (var i in cmd.Items)
            {
                if (i.Quantity <= 0)
                {
                    error = new(ShoppingListErrors.ItemQuantityZeroOrLess, i.ItemName);
                    return false;
                }
            }
        }

        error = null;
        return true;
    }

    private static bool ValidateItemsNotRepeated(UpdateShoppingListCommand cmd, out CqrsError? error)
    {
        if (cmd.Items != null)
        {
            bool hasRepeatedItem = cmd.Items.GroupBy(i => i.ItemName, i => i)
                                             .Select(d => (d.Key, d.Count()))
                                             .Any(d => d.Item2 > 1);
            if (hasRepeatedItem)
            {
                error = new(ShoppingListErrors.RepeatedItems);
                return false;
            }
        }

        error = null;
        return true;
    }
}