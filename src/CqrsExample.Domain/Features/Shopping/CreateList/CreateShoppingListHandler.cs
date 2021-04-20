using System.Threading.Tasks;
using CqrsExample.Domain.BaseAbstractions.Errors;
using CqrsExample.Domain.BaseAbstractions.Commands;
using CqrsExample.Domain.Features.Shopping.Entities;
using CqrsExample.Domain.Features.Shopping.Repositories;

namespace CqrsExample.Domain.Features.Shopping.CreateList
{
    public sealed class CreateShoppingListHandler : ICommandHandler<CreateShoppingListCommand, CreateShoppingListResult>
    {
        private static readonly Error errorInternal = new Error(ShoppingListErrors.InternalError);

        private readonly IShoppingListWriteRepository repository;

        public CreateShoppingListHandler(IShoppingListWriteRepository repository) =>
            this.repository = repository;

        public async Task<CommandResult<CreateShoppingListResult>> HandleAsync(CreateShoppingListCommand cmd)
        {
            if (!cmd.Validate(out var errors))
                return CommandResult<CreateShoppingListResult>.Fail(CommandResultFailureType.Validation, errors!);

            var shoppingList = new ShoppingList(cmd.Title!);

            var success = await repository.InsertAsync(shoppingList);

            return success ?
                CommandResult<CreateShoppingListResult>.Success(new CreateShoppingListResult(shoppingList)) :
                CommandResult<CreateShoppingListResult>.Fail(CommandResultFailureType.InternalError, errorInternal);
        }
    }
}