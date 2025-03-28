using CqrsExample.Domain.BaseAbstractions;
using CqrsExample.Domain.Features.Shopping.Common.Entities;
using CqrsExample.Domain.Features.Shopping.Common.Repositories;

namespace CqrsExample.Domain.Features.Shopping.UpdateList;

public sealed record UpdateShoppingListCommand(
    Guid? Id,
    string? Title,
    ShoppingListItem[]? Items) : Command;

public sealed record UpdateShoppingListResult(
    CommandResultFailureType? FailureType = null,
    CqrsError? Error = null) : CommandResult(FailureType, Error)
{
    public static readonly UpdateShoppingListResult Success = new(null, null);
    public static readonly UpdateShoppingListResult NotFound = new(CommandResultFailureType.NotFound, new(ShoppingListErrors.ShoppingListNotFound));
    public static readonly UpdateShoppingListResult InternalError = new(CommandResultFailureType.InternalError, new(ShoppingListErrors.InternalError));
}

public sealed class UpdateShoppingListHandler(IShoppingListWriteRepository repo)
{
    public async Task<UpdateShoppingListResult> HandleAsync(UpdateShoppingListCommand cmd)
    {
        if (!cmd.Validate(out var error))
            return new(CommandResultFailureType.Validation, error!);

        var shoppingList = await repo.GetAsync((Guid)cmd.Id!);

        if (shoppingList == null)
            return UpdateShoppingListResult.NotFound;

        shoppingList.Update(cmd.Title!, cmd.Items!);

        bool success = await repo.UpdateAsync(shoppingList);

        return success ?
            UpdateShoppingListResult.Success :
            UpdateShoppingListResult.InternalError;
    }
}