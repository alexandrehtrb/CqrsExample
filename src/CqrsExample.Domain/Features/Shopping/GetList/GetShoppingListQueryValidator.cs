using CqrsExample.Domain.BaseAbstractions;

namespace CqrsExample.Domain.Features.Shopping.GetList;

public static class GetShoppingListQueryValidator
{
    public static bool Validate(this GetShoppingListQuery qry, out CqrsError? error) =>
        ValidateId(qry, out error);

    private static bool ValidateId(GetShoppingListQuery qry, out CqrsError? error)
    {
        if (qry.Id == null || qry.Id == Guid.Empty)
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
}