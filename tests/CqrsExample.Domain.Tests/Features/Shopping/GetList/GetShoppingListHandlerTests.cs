using CqrsExample.Domain.BaseAbstractions;
using CqrsExample.Domain.Features.Shopping;
using CqrsExample.Domain.Features.Shopping.Common.Entities;
using CqrsExample.Domain.Features.Shopping.Common.Repositories;
using CqrsExample.Domain.Features.Shopping.GetList;
using NSubstitute;
using Xunit;

namespace CqrsExample.Domain.Tests.Features.Shopping.GetList;

public class GetShoppingListHandlerTests
{
    private static readonly Guid testId = Guid.NewGuid();
    private static readonly GetShoppingListQuery invalidQry = new(Guid.Empty);
    private static readonly GetShoppingListQuery validQry = new(testId);
    private static readonly ShoppingList testList = new()
    {
        Id = testId,
        Title = "Minha lista de compras",
        Items = Array.Empty<ShoppingListItem>()
    };

    private readonly IShoppingListReadRepository repo;
    private readonly GetShoppingListHandler handler;

    public GetShoppingListHandlerTests()
    {
        this.repo = Substitute.For<IShoppingListReadRepository>();
        this.handler = new(this.repo);
    }

    [Fact]
    public async Task Should_fail_validation_when_query_is_invalid()
    {
        // GIVEN
        // WHEN
        var result = await this.handler.HandleAsync(invalidQry);

        // THEN
        await this.repo.DidNotReceiveWithAnyArgs().GetAsync(default!);
        Assert.False(result.Successful);
        Assert.Equal(QueryResultFailureType.Validation, result.FailureType);
        Assert.Equal(new(ShoppingListErrors.InvalidId), result.Error!);
    }

    [Fact]
    public async Task Should_fail_not_found_when_list_was_not_found()
    {
        // GIVEN
        this.repo.GetAsync((Guid)validQry.Id!).Returns((ShoppingList?)null);

        // WHEN
        var result = await this.handler.HandleAsync(validQry);

        // THEN
        await this.repo.Received().GetAsync((Guid)validQry.Id!);
        Assert.False(result.Successful);
        Assert.Equal(QueryResultFailureType.NotFound, result.FailureType);
        Assert.Equal(new(ShoppingListErrors.ShoppingListNotFound), result.Error!);
    }

    [Fact]
    public async Task Should_be_successful_if_list_is_found()
    {
        // GIVEN
        this.repo.GetAsync((Guid)validQry.Id!).Returns(testList);

        // WHEN
        var result = await this.handler.HandleAsync(validQry);

        // THEN
        await this.repo.Received().GetAsync((Guid)validQry.Id!);
        Assert.True(result.Successful);
        Assert.Equal(testList.Id, result.Id);
        Assert.Equal(testList.Title, result.Title);
        Assert.Equal(testList.Items, result.Items);
    }
}