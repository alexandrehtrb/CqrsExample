using CqrsExample.Domain.BaseAbstractions;
using CqrsExample.Domain.Features.Shopping;
using CqrsExample.Domain.Features.Shopping.Common.Entities;
using CqrsExample.Domain.Features.Shopping.Common.Repositories;
using CqrsExample.Domain.Features.Shopping.UpdateList;
using NSubstitute;
using Xunit;

namespace CqrsExample.Domain.Tests.Features.Shopping.UpdateList;

public class UpdateShoppingListHandlerTests
{
    private static readonly UpdateShoppingListCommand invalidCmd = new(null, null, null);
    private static readonly UpdateShoppingListCommand validCmd = CreateValidTestCmd();
    private static readonly ShoppingList testShoppingList = new()
    {
        Id = (Guid)validCmd.Id!,
        Title = "Lista de compras",
        Items = Array.Empty<ShoppingListItem>()
    };
    private static readonly ShoppingList testShoppingListUpdated = new()
    {
        Id = (Guid)validCmd.Id!,
        Title = validCmd.Title!,
        Items = validCmd.Items!
    };

    private readonly IShoppingListWriteRepository repo;
    private readonly UpdateShoppingListHandler handler;

    public UpdateShoppingListHandlerTests()
    {
        this.repo = Substitute.For<IShoppingListWriteRepository>();
        this.handler = new(this.repo);
    }

    [Fact]
    public async Task Should_fail_validation_when_command_is_invalid()
    {
        // GIVEN
        // WHEN
        var result = await this.handler.HandleAsync(invalidCmd);

        // THEN
        await this.repo.DidNotReceiveWithAnyArgs().GetAsync(default);
        await this.repo.DidNotReceiveWithAnyArgs().UpdateAsync(null!);
        Assert.False(result.Successful);
        Assert.Equal(CommandResultFailureType.Validation, result.FailureType);
        Assert.Equal(new(ShoppingListErrors.InvalidId), result.Error);
    }

    [Fact]
    public async Task Should_fail_not_found_if_list_was_not_found()
    {
        // GIVEN
        this.repo.GetAsync((Guid)validCmd.Id!)
                 .Returns((ShoppingList?)null);

        // WHEN
        var result = await this.handler.HandleAsync(validCmd);

        // THEN
        await this.repo.Received().GetAsync((Guid)validCmd.Id!);
        await this.repo.DidNotReceiveWithAnyArgs().UpdateAsync(null!);
        Assert.False(result.Successful);
        Assert.Equal(CommandResultFailureType.NotFound, result.FailureType);
        Assert.Equal(new(ShoppingListErrors.ShoppingListNotFound), result.Error);
    }

    [Fact]
    public async Task Should_fail_internal_error_when_list_could_not_be_saved()
    {
        // GIVEN
        this.repo.GetAsync((Guid)validCmd.Id!)
                 .Returns(testShoppingList);
        this.repo.UpdateAsync(testShoppingListUpdated)
                 .Returns(false);

        // WHEN
        var result = await this.handler.HandleAsync(validCmd);

        // THEN
        await this.repo.Received().GetAsync((Guid)validCmd.Id!);
        await this.repo.Received().UpdateAsync(testShoppingListUpdated);
        Assert.False(result.Successful);
        Assert.Equal(CommandResultFailureType.InternalError, result.FailureType);
        Assert.Equal(new(ShoppingListErrors.InternalError), result.Error);
    }

    [Fact]
    public async Task Should_be_successful_if_list_was_updated_in_database()
    {
        // GIVEN
        this.repo.GetAsync((Guid)validCmd.Id!)
                 .Returns(testShoppingList);
        this.repo.UpdateAsync(testShoppingListUpdated)
                 .Returns(true);

        // WHEN
        var result = await this.handler.HandleAsync(validCmd);

        // THEN
        await this.repo.Received().GetAsync((Guid)validCmd.Id!);
        await this.repo.Received().UpdateAsync(testShoppingListUpdated);
        Assert.True(result.Successful);
        Assert.Equal(UpdateShoppingListResult.Success, result);
    }

    private static UpdateShoppingListCommand CreateValidTestCmd() => new(
        Id: Guid.NewGuid(),
        Title: "Minha lista de compras",
        Items: Array.Empty<ShoppingListItem>()
    );
}