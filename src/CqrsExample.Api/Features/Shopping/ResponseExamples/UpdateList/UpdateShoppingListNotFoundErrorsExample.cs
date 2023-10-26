using CqrsExample.Domain.BaseAbstractions.Errors;
using CqrsExample.Domain.Features.Shopping;
using Swashbuckle.AspNetCore.Filters;

namespace CqrsExample.Api.Features.Shopping.ResponseExamples.UpdateList;

public class UpdateShoppingListNotFoundErrorsExample : IExamplesProvider<Error[]>
{
    public Error[] GetExamples() =>
        new[] {
            new Error(ShoppingListErrors.ShoppingListNotFound)
        };
}