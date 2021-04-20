using System;
using System.Threading.Tasks;
using CqrsExample.Domain.BaseAbstractions.Errors;
using CqrsExample.Domain.BaseAbstractions.Queries;
using CqrsExample.Domain.Features.Shopping;
using CqrsExample.Domain.Features.Shopping.Entities;
using CqrsExample.Domain.Features.Shopping.GetList;
using CqrsExample.Domain.Features.Shopping.Repositories;
using Moq;
using Xunit;

namespace CqrsExample.Domain.Tests.Feature.Shopping.GetList
{
    public class GetShoppingListHandlerTests
    {
        private static readonly Guid testId = Guid.NewGuid();
        private static readonly GetShoppingListQuery invalidQry = new GetShoppingListQuery(Guid.Empty);
        private static readonly GetShoppingListQuery validQry = new GetShoppingListQuery(testId);
        private static readonly ShoppingList testShoppingList = new ShoppingList()
        {
            Id = testId, Title = "Minha lista de compras", Items = new ShoppingListItem[0]
        };
        private static readonly GetShoppingListResult testResult = new GetShoppingListResult()
        {
            Id = testId, Title = "Minha lista de compras", Items = new ShoppingListItem[0]
        };

        private readonly Mock<IShoppingListReadRepository> repository;
        private readonly GetShoppingListHandler handler;

        public GetShoppingListHandlerTests()
        {
            repository = new Mock<IShoppingListReadRepository>();
            handler = new GetShoppingListHandler(repository.Object);
        }

        [Fact]
        public async Task Should_fail_validation_when_query_is_invalid()
        {
            // GIVEN
            // WHEN
            var result = await handler.HandleAsync(invalidQry);

            // THEN
            repository.VerifyNoOtherCalls();
            Assert.False(result.IsSuccess);
            Assert.Equal(QueryResultFailureType.Validation, result.FailureType);
            Assert.Single(result.Errors, new Error(ShoppingListErrors.InvalidId));
        }

        [Fact]
        public async Task Should_fail_not_found_when_list_was_not_found()
        {
            // GIVEN
            repository.Setup(r => r.QueryAsync<GetShoppingListResult>((Guid) validQry.Id!))
                      .ReturnsAsync((GetShoppingListResult?) null);

            // WHEN
            var result = await handler.HandleAsync(validQry);

            // THEN
            repository.Verify(r => r.QueryAsync<GetShoppingListResult>((Guid) validQry.Id!), Times.Once);
            Assert.False(result.IsSuccess);
            Assert.Equal(QueryResultFailureType.NotFound, result.FailureType);
            Assert.Single(result.Errors, new Error(ShoppingListErrors.ShoppingListNotFound));
        }

        [Fact]
        public async Task Should_be_successful_if_list_is_found()
        {
            // GIVEN
            repository.Setup(r => r.QueryAsync<GetShoppingListResult>((Guid) validQry.Id!))
                      .ReturnsAsync(testResult);

            // WHEN
            var result = await handler.HandleAsync(validQry);

            // THEN
            repository.Verify(r => r.QueryAsync<GetShoppingListResult>((Guid) validQry.Id!), Times.Once);
            Assert.True(result.IsSuccess);
            Assert.Equal(testResult, result.SuccessResult);
        }
    }
}