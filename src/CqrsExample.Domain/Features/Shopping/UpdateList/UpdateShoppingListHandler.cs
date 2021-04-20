using System;
using System.Threading.Tasks;
using CqrsExample.Domain.BaseAbstractions.Errors;
using CqrsExample.Domain.BaseAbstractions.Commands;
using CqrsExample.Domain.Features.Shopping.Repositories;

namespace CqrsExample.Domain.Features.Shopping.UpdateList
{
    public sealed class UpdateShoppingListHandler : ICommandHandler<UpdateShoppingListCommand, UpdateShoppingListResult>
    {
        private static readonly Error errorInternal = new Error(ShoppingListErrors.InternalError);
        private static readonly Error errorNotFound = new Error(ShoppingListErrors.ShoppingListNotFound);

        private readonly IShoppingListWriteRepository repository;

        public UpdateShoppingListHandler(IShoppingListWriteRepository repository) =>
            this.repository = repository;

        public async Task<CommandResult<UpdateShoppingListResult>> HandleAsync(UpdateShoppingListCommand cmd)
        {
            if (!cmd.Validate(out var errors))
                return CommandResult<UpdateShoppingListResult>.Fail(CommandResultFailureType.Validation, errors!);

            var shoppingList = await repository.GetAsync((Guid) cmd.GetId()!);
            if (shoppingList == null)
                return CommandResult<UpdateShoppingListResult>.Fail(CommandResultFailureType.NotFound, errorNotFound);

            shoppingList.Update(cmd.Title!, cmd.Items!);

            var success = await repository.UpdateAsync(shoppingList);

            return success ?
                CommandResult<UpdateShoppingListResult>.Success(new UpdateShoppingListResult()) :
                CommandResult<UpdateShoppingListResult>.Fail(CommandResultFailureType.InternalError, errorInternal);
        }
    }
}