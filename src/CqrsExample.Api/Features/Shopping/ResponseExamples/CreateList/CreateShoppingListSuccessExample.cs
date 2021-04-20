using System;
using CqrsExample.Domain.Features.Shopping.Entities;
using CqrsExample.Domain.Features.Shopping.CreateList;
using Swashbuckle.AspNetCore.Filters;

namespace CqrsExample.Api.Features.Shopping.ResponseExamples.CreateList
{
    public class CreateShoppingListSuccessExample : IExamplesProvider<CreateShoppingListResult>
    {
        public CreateShoppingListResult GetExamples() =>
            new CreateShoppingListResult(
                new ShoppingList()
                {
                    Id = Guid.NewGuid(),
                    Title = "My shopping list",
                    Items = new ShoppingListItem[0]
                });
    }
}