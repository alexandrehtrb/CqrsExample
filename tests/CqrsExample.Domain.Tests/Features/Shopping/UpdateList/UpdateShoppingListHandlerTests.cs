using System.Threading.Tasks;
using CqrsExample.Domain.BaseAbstractions.Errors;
using CqrsExample.Domain.BaseAbstractions.Commands;
using CqrsExample.Domain.Features.Shopping;
using CqrsExample.Domain.Features.Shopping.Entities;
using CqrsExample.Domain.Features.Shopping.UpdateList;
using CqrsExample.Domain.Features.Shopping.Repositories;
using Moq;
using Xunit;
using System;

namespace CqrsExample.Domain.Tests.Feature.Shopping.UpdateList
{
    public class UpdateShoppingListHandlerTests
    {
        private static readonly UpdateShoppingListCommand invalidCmd = new UpdateShoppingListCommand();
        private static readonly UpdateShoppingListCommand validCmd = CreateValidTestCmd();
        private static readonly ShoppingList testShoppingList = new ShoppingList()
        {
            Id = (Guid) validCmd.GetId()!, Title = "Lista de compras", Items = new ShoppingListItem[0]
        };
        private static readonly ShoppingList testShoppingListUpdated = new ShoppingList()
        {
            Id = (Guid) validCmd.GetId()!, Title = validCmd.Title!, Items = validCmd.Items!
        };

        private readonly Mock<IShoppingListWriteRepository> repository;
        private readonly UpdateShoppingListHandler handler;

        public UpdateShoppingListHandlerTests()
        {
            repository = new Mock<IShoppingListWriteRepository>();
            handler = new UpdateShoppingListHandler(repository.Object);
        }

        [Fact]
        public async Task Should_fail_validation_when_command_is_invalid()
        {
            // GIVEN
            // WHEN
            var result = await handler.HandleAsync(invalidCmd);

            // THEN
            repository.VerifyNoOtherCalls();
            Assert.False(result.IsSuccess);
            Assert.Equal(CommandResultFailureType.Validation, result.FailureType);
            Assert.NotNull(result.Errors);
            Assert.Equal(3, result.Errors!.Length);
            Assert.Contains(new Error(ShoppingListErrors.InvalidId), result.Errors);
            Assert.Contains(new Error(ShoppingListErrors.BlankTitle), result.Errors);
            Assert.Contains(new Error(ShoppingListErrors.ItemsNull), result.Errors);
        }

        [Fact]
        public async Task Should_fail_not_found_if_list_was_not_found()
        {
            // GIVEN
            repository.Setup(r => r.GetAsync((Guid) validCmd.GetId()!))
                      .ReturnsAsync((ShoppingList?) null);

            // WHEN
            var result = await handler.HandleAsync(validCmd);

            // THEN
            repository.Verify(r => r.GetAsync((Guid) validCmd.GetId()!), Times.Once);
            repository.Verify(r => r.UpdateAsync(It.IsAny<ShoppingList>()), Times.Never);
            Assert.False(result.IsSuccess);
            Assert.Equal(CommandResultFailureType.NotFound, result.FailureType);
            Assert.Single(result.Errors, new Error(ShoppingListErrors.ShoppingListNotFound));
        }

        [Fact]
        public async Task Should_fail_internal_error_when_list_could_not_be_saved()
        {
            // GIVEN
            repository.Setup(r => r.GetAsync((Guid) validCmd.GetId()!))
                      .ReturnsAsync(testShoppingList);
            repository.Setup(r => r.UpdateAsync(testShoppingListUpdated))
                      .ReturnsAsync(false);

            // WHEN
            var result = await handler.HandleAsync(validCmd);

            // THEN
            repository.Verify(r => r.GetAsync((Guid) validCmd.GetId()!), Times.Once);
            repository.Verify(r => r.UpdateAsync(testShoppingListUpdated), Times.Once);
            Assert.False(result.IsSuccess);
            Assert.Equal(CommandResultFailureType.InternalError, result.FailureType);
            Assert.Single(result.Errors, new Error(ShoppingListErrors.InternalError));
        }

        [Fact]
        public async Task Should_be_successful_if_list_was_updated_in_database()
        {
            // GIVEN
            repository.Setup(r => r.GetAsync((Guid) validCmd.GetId()!))
                      .ReturnsAsync(testShoppingList);
            repository.Setup(r => r.UpdateAsync(testShoppingListUpdated))
                      .ReturnsAsync(true);

            // WHEN
            var result = await handler.HandleAsync(validCmd);

            // THEN
            repository.Verify(r => r.GetAsync((Guid) validCmd.GetId()!), Times.Once);
            repository.Verify(r => r.UpdateAsync(testShoppingListUpdated), Times.Once);
            Assert.True(result.IsSuccess);
            Assert.Equal(new UpdateShoppingListResult(), result.SuccessResult);
        }

        private static UpdateShoppingListCommand CreateValidTestCmd()
        {
            var cmd = new UpdateShoppingListCommand()
            {
                Title = "Minha lista de compras",
                Items = new ShoppingListItem[0]
            };
            cmd.SetId(Guid.NewGuid());
            return cmd;
        }
    }
}