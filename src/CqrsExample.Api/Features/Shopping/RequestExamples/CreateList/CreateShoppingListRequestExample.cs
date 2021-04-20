using CqrsExample.Domain.Features.Shopping.CreateList;
using Swashbuckle.AspNetCore.Filters;

namespace CqrsExample.Api.Features.Shopping.RequestExamples.CreateList
{
    public class CreateShoppingListRequestExample : IExamplesProvider<CreateShoppingListCommand>
    {
        public CreateShoppingListCommand GetExamples() =>
            new CreateShoppingListCommand() { Title = "My shopping list" };
    }
}