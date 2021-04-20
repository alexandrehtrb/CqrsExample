using System;
using CqrsExample.Domain.Features.Shopping.Entities;
using CqrsExample.Domain.Features.Shopping.GetList;
using Swashbuckle.AspNetCore.Filters;

namespace CqrsExample.Api.Features.Shopping.ResponseExamples.GetList
{
    public class GetShoppingListSuccessExample : IExamplesProvider<GetShoppingListResult>
    {
        public GetShoppingListResult GetExamples() =>
            new GetShoppingListResult()
                {
                    Id = Guid.NewGuid(),
                    Title = "My shopping list",
                    Items = new []
                    {
                        new ShoppingListItem(1, "Rice 5kg"),
                        new ShoppingListItem(2, "Beans 1kg")
                    }
                };
    }
}