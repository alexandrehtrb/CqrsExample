using CqrsExample.Domain.BaseAbstractions;

namespace CqrsExample.Domain.Features.Shopping.CreateList;

public static class CreateShoppingListCommandValidator
{
    public static bool Validate(this CreateShoppingListCommand cmd, out CqrsError? error) =>
        ValidateTitle(cmd, out error);

    private static bool ValidateTitle(CreateShoppingListCommand cmd, out CqrsError? error)
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
}