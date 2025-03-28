using CqrsExample.Domain.BaseAbstractions;
using CqrsExample.Domain.Features.Shopping.Common.Entities;
using CqrsExample.Domain.Features.Shopping.Common.Repositories;

namespace CqrsExample.Domain.Features.Shopping.CreateList;

public sealed record CreateShoppingListCommand(string? Title) : Command;

public sealed record CreateShoppingListResult(
    Guid Id,
    string Title,
    ShoppingListItem[] Items,
    CommandResultFailureType? FailureType = null,
    CqrsError? Error = null) : CommandResult(FailureType, Error)
{
    private static CreateShoppingListResult Failed(CommandResultFailureType failureType, CqrsError error) => new(
        Guid.Empty,
        string.Empty,
        [],
        failureType,
        error);

    public static readonly CreateShoppingListResult InternalError =
        Failed(CommandResultFailureType.InternalError, new(ShoppingListErrors.InternalError));

    public static CreateShoppingListResult ValidationError(CqrsError error) =>
        Failed(CommandResultFailureType.Validation, error);

    public static CreateShoppingListResult Success(ShoppingList list) => new(
        Id: list.Id,
        Title: list.Title,
        Items: list.Items
    );
}

public sealed class CreateShoppingListHandler(IShoppingListWriteRepository repo)
{
    public async Task<CreateShoppingListResult> HandleAsync(CreateShoppingListCommand cmd)
    {
        if (!cmd.Validate(out var error))
            return CreateShoppingListResult.ValidationError(error!);

        var shoppingList = new ShoppingList(cmd.Title!);

        bool success = await repo.InsertAsync(shoppingList);

        return success ?
            CreateShoppingListResult.Success(shoppingList) :
            CreateShoppingListResult.InternalError;
    }
}