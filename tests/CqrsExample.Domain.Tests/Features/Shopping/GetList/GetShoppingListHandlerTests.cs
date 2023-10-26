using System;
using System.Threading.Tasks;
using CqrsExample.Domain.BaseAbstractions.Errors;
using CqrsExample.Domain.BaseAbstractions.Queries;
using CqrsExample.Domain.Features.Shopping;
using CqrsExample.Domain.Features.Shopping.Common.Entities;
using CqrsExample.Domain.Features.Shopping.GetList;
using CqrsExample.Domain.Features.Shopping.Common.Repositories;
using Moq;
using Xunit;

namespace CqrsExample.Domain.Tests.Feature.Shopping.GetList;

public class GetShoppingListHandlerTests
{
    private static readonly Guid testId = Guid.NewGuid();
    private static readonly GetShoppingListQuery invalidQry = new(Guid.Empty);
    private static readonly GetShoppingListQuery validQry = new(testId);
    private static readonly ShoppingList testShoppingList = new()
    {
        Id = testId,
        Title = "Minha lista de compras",
        Items = Array.Empty<ShoppingListItem>()
    };
    private static readonly GetShoppingListResult testResult = new()
    {
        Id = testId,
        Title = "Minha lista de compras",
        Items = Array.Empty<ShoppingListItem>()
    };

    private readonly Mock<IShoppingListReadRepository> repository;
    private readonly GetShoppingListHandler handler;

    public GetShoppingListHandlerTests()
    {
        this.repository = new Mock<IShoppingListReadRepository>();
        this.handler = new GetShoppingListHandler(this.repository.Object);
    }

    [Fact]
    public async Task Should_fail_validation_when_query_is_invalid()
    {
        // GIVEN
        // WHEN
        var result = await this.handler.HandleAsync(invalidQry);

        // THEN
        this.repository.VerifyNoOtherCalls();
        Assert.False(result.IsSuccess);
        Assert.Equal(QueryResultFailureType.Validation, result.FailureType);
        Assert.Single(result.Errors!, new Error(ShoppingListErrors.InvalidId));
    }

    [Fact]
    public async Task Should_fail_not_found_when_list_was_not_found()
    {
        // GIVEN
        this.repository.Setup(r => r.QueryAsync<GetShoppingListResult>((Guid)validQry.Id!))
                  .ReturnsAsync((GetShoppingListResult?)null);

        // WHEN
        var result = await this.handler.HandleAsync(validQry);

        // THEN
        this.repository.Verify(r => r.QueryAsync<GetShoppingListResult>((Guid)validQry.Id!), Times.Once);
        Assert.False(result.IsSuccess);
        Assert.Equal(QueryResultFailureType.NotFound, result.FailureType);
        Assert.Single(result.Errors!, new Error(ShoppingListErrors.ShoppingListNotFound));
    }

    [Fact]
    public async Task Should_be_successful_if_list_is_found()
    {
        // GIVEN
        this.repository.Setup(r => r.QueryAsync<GetShoppingListResult>((Guid)validQry.Id!))
                  .ReturnsAsync(testResult);

        // WHEN
        var result = await this.handler.HandleAsync(validQry);

        // THEN
        this.repository.Verify(r => r.QueryAsync<GetShoppingListResult>((Guid)validQry.Id!), Times.Once);
        Assert.True(result.IsSuccess);
        Assert.Equal(testResult, result.SuccessResult);
    }
}