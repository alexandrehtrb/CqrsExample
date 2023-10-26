using System;
using CqrsExample.Domain.Features.Shopping.CreateList;
using CqrsExample.Domain.Features.Shopping.Entities;
using Swashbuckle.AspNetCore.Filters;

namespace CqrsExample.Api.Features.Shopping.ResponseExamples.CreateList;

public class CreateShoppingListSuccessExample : IExamplesProvider<CreateShoppingListResult>
{
    public CreateShoppingListResult GetExamples() =>
        new(
            new ShoppingList()
            {
                Id = Guid.NewGuid(),
                Title = "My shopping list",
                Items = Array.Empty<ShoppingListItem>()
            });
}