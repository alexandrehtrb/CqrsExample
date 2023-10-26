using System.Collections.Generic;
using CqrsExample.Domain.BaseAbstractions.Errors;

namespace CqrsExample.Domain.Features.Shopping.CreateList;

public static class CreateShoppingListCommandValidator
{
    public static bool Validate(this CreateShoppingListCommand cmd, out Error[]? errors)
    {
        var errorsList = new List<Error>();

        ValidateTitle(cmd, errorsList);

        errors = errorsList.Count > 0 ? errorsList.ToArray() : null;
        return errorsList.Count == 0;
    }

    private static void ValidateTitle(CreateShoppingListCommand cmd, IList<Error> errorsList)
    {
        if (string.IsNullOrWhiteSpace(cmd.Title))
            errorsList.Add(new Error(ShoppingListErrors.BlankTitle));
    }
}