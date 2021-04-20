using CqrsExample.Domain.BaseAbstractions.Errors;
using CqrsExample.Domain.Features.Shopping;
using Swashbuckle.AspNetCore.Filters;

namespace CqrsExample.Api.Features.Shopping.ResponseExamples.UpdateList
{
    public class UpdateShoppingListValidationErrorsExample : IExamplesProvider<Error[]>
    {
        public Error[] GetExamples() =>
            new [] {
                new Error(ShoppingListErrors.InvalidId),
                new Error(ShoppingListErrors.BlankTitle),
                new Error(ShoppingListErrors.ItemsNull),
                new Error(ShoppingListErrors.BlankItemName),
                new Error(ShoppingListErrors.ItemQuantityZeroOrLess, "Rice 5kg"),
                new Error(ShoppingListErrors.RepeatedItems),
            };
    }
}