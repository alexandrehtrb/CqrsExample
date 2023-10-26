using CqrsExample.Domain.Features.Shopping.Common.Entities;
using CqrsExample.Domain.Features.Shopping.UpdateList;
using Swashbuckle.AspNetCore.Filters;

namespace CqrsExample.Api.Features.Shopping.RequestExamples.UpdateList;

public class UpdateShoppingListRequestExample : IExamplesProvider<UpdateShoppingListCommand>
{
    public UpdateShoppingListCommand GetExamples() =>
        new()
        {
            Title = "My shopping list",
            Items = new[]
            {
                new ShoppingListItem(1, "Rice 5kg"),
                new ShoppingListItem(2, "Beans 1kg")
            }
        };
}