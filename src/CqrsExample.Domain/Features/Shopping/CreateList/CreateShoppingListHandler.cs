using System.Threading.Tasks;
using CqrsExample.Domain.BaseAbstractions.Commands;
using CqrsExample.Domain.BaseAbstractions.Errors;
using CqrsExample.Domain.Features.Shopping.Common.Entities;
using CqrsExample.Domain.Features.Shopping.Common.Repositories;

namespace CqrsExample.Domain.Features.Shopping.CreateList;

public sealed class CreateShoppingListHandler : ICommandHandler<CreateShoppingListCommand, CreateShoppingListResult>
{
    private static readonly Error errorInternal = new(ShoppingListErrors.InternalError);

    private readonly IShoppingListWriteRepository repository;

    public CreateShoppingListHandler(IShoppingListWriteRepository repository) =>
        this.repository = repository;

    public async Task<CommandResult<CreateShoppingListResult>> HandleAsync(CreateShoppingListCommand cmd)
    {
        if (!cmd.Validate(out var errors))
            return CommandResult<CreateShoppingListResult>.Fail(CommandResultFailureType.Validation, errors!);

        var shoppingList = new ShoppingList(cmd.Title!);

        bool success = await this.repository.InsertAsync(shoppingList);

        return success ?
            CommandResult<CreateShoppingListResult>.Success(new CreateShoppingListResult(shoppingList)) :
            CommandResult<CreateShoppingListResult>.Fail(CommandResultFailureType.InternalError, errorInternal);
    }
}