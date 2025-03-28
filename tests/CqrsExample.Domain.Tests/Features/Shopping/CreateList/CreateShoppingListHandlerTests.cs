using CqrsExample.Domain.BaseAbstractions;
using CqrsExample.Domain.Features.Shopping;
using CqrsExample.Domain.Features.Shopping.Common.Entities;
using CqrsExample.Domain.Features.Shopping.Common.Repositories;
using CqrsExample.Domain.Features.Shopping.CreateList;
using NSubstitute;
using Xunit;

namespace CqrsExample.Domain.Tests.Features.Shopping.CreateList;

public class CreateShoppingListHandlerTests
{
    private static readonly string testTitle = "Lista de compras 2";
    private static readonly CreateShoppingListCommand invalidCmd = new(null);
    private static readonly CreateShoppingListCommand validCmd = new(testTitle);

    private readonly IShoppingListWriteRepository repo;
    private readonly CreateShoppingListHandler handler;

    public CreateShoppingListHandlerTests()
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
        await this.repo.DidNotReceiveWithAnyArgs().InsertAsync(null!);
        Assert.False(result.Successful);
        Assert.Equal(CommandResultFailureType.Validation, result.FailureType);
        Assert.Equal(new(ShoppingListErrors.BlankTitle), result.Error);
    }

    [Fact]
    public async Task Should_fail_internal_error_when_list_could_not_be_saved()
    {
        // GIVEN
        this.repo.InsertAsync(Arg.Is<ShoppingList>(s => s.Title == testTitle))
                 .Returns(false);

        // WHEN
        var result = await this.handler.HandleAsync(validCmd);

        // THEN
        await this.repo.Received().InsertAsync(Arg.Is<ShoppingList>(s => s.Title == testTitle));
        Assert.False(result.Successful);
        Assert.Equal(CommandResultFailureType.InternalError, result.FailureType);
        Assert.Equal(new(ShoppingListErrors.InternalError), result.Error);
    }

    [Fact]
    public async Task Should_be_successful_if_list_was_saved()
    {
        // GIVEN
        this.repo.InsertAsync(Arg.Is<ShoppingList>(s => s.Title == testTitle))
                 .Returns(true);

        // WHEN
        var result = await this.handler.HandleAsync(validCmd);

        // THEN
        await this.repo.Received().InsertAsync(Arg.Is<ShoppingList>(s => s.Title == testTitle));
        Assert.True(result.Successful);
        Assert.NotEqual(Guid.Empty, result.Id);
        Assert.Equal(testTitle, result.Title);
        Assert.Empty(result.Items);
    }
}