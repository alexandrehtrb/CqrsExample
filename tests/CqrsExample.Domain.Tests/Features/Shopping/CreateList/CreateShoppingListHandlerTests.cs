using System;
using System.Threading.Tasks;
using CqrsExample.Domain.BaseAbstractions.Commands;
using CqrsExample.Domain.BaseAbstractions.Errors;
using CqrsExample.Domain.Features.Shopping;
using CqrsExample.Domain.Features.Shopping.CreateList;
using CqrsExample.Domain.Features.Shopping.Common.Entities;
using CqrsExample.Domain.Features.Shopping.Common.Repositories;
using Moq;
using Xunit;

namespace CqrsExample.Domain.Tests.Feature.Shopping.CreateList;

public class CreateShoppingListHandlerTests
{
    private static readonly string testTitle = "Lista de compras 2";
    private static readonly CreateShoppingListCommand invalidCmd = new();
    private static readonly CreateShoppingListCommand validCmd = new() { Title = testTitle };

    private readonly Mock<IShoppingListWriteRepository> repository;
    private readonly CreateShoppingListHandler handler;

    public CreateShoppingListHandlerTests()
    {
        this.repository = new Mock<IShoppingListWriteRepository>();
        this.handler = new CreateShoppingListHandler(this.repository.Object);
    }

    [Fact]
    public async Task Should_fail_validation_when_command_is_invalid()
    {
        // GIVEN
        // WHEN
        var result = await this.handler.HandleAsync(invalidCmd);

        // THEN
        this.repository.VerifyNoOtherCalls();
        Assert.False(result.IsSuccess);
        Assert.Equal(CommandResultFailureType.Validation, result.FailureType);
        Assert.Single(result.Errors!, new Error(ShoppingListErrors.BlankTitle));
    }

    [Fact]
    public async Task Should_fail_internal_error_when_list_could_not_be_saved()
    {
        // GIVEN
        this.repository.Setup(r => r.InsertAsync(It.Is<ShoppingList>(s => s.Title == testTitle)))
                  .ReturnsAsync(false);

        // WHEN
        var result = await this.handler.HandleAsync(validCmd);

        // THEN
        this.repository.Verify(r => r.InsertAsync(It.Is<ShoppingList>(s => s.Title == testTitle)), Times.Once);
        Assert.False(result.IsSuccess);
        Assert.Equal(CommandResultFailureType.InternalError, result.FailureType);
        Assert.Single(result.Errors!, new Error(ShoppingListErrors.InternalError));
    }

    [Fact]
    public async Task Should_be_successful_if_list_was_saved()
    {
        // GIVEN
        this.repository.Setup(r => r.InsertAsync(It.Is<ShoppingList>(s => s.Title == testTitle)))
                  .ReturnsAsync(true);

        // WHEN
        var result = await this.handler.HandleAsync(validCmd);

        // THEN
        this.repository.Verify(r => r.InsertAsync(It.Is<ShoppingList>(s => s.Title == testTitle)), Times.Once);
        Assert.True(result.IsSuccess);
        Assert.NotNull(result.SuccessResult);
        Assert.NotEqual(Guid.Empty, result.SuccessResult!.Id);
        Assert.Equal(testTitle, result.SuccessResult!.Title);
        Assert.Empty(result.SuccessResult!.Items);
    }
}