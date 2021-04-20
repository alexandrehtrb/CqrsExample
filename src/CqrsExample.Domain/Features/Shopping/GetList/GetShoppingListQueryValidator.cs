using System;
using System.Collections.Generic;
using CqrsExample.Domain.BaseAbstractions.Errors;

namespace CqrsExample.Domain.Features.Shopping.GetList
{
    public static class GetShoppingListQueryValidator
    {
        public static bool Validate(this GetShoppingListQuery qry, out Error[]? errors)
        {
            var errorsList = new List<Error>();

            ValidateId(qry, errorsList);

            errors = errorsList.Count > 0 ? errorsList.ToArray() : null;
            return errorsList.Count == 0;
        }

        private static void ValidateId(GetShoppingListQuery qry, IList<Error> errorsList)
        {
            if (qry.Id == null || qry.Id == Guid.Empty)
                errorsList.Add(new Error(ShoppingListErrors.InvalidId));
        }
    }
}