using CqrsExample.Domain.BaseAbstractions.Errors;
using CqrsExample.Domain.Features.Shopping;
using CqrsExample.Domain.Features.Shopping.CreateList;
using Xunit;

namespace CqrsExample.Domain.Tests.Feature.Shopping.CreateList
{
    public static class CreateShoppingListCommandValidatorTests
    {
        [Theory]
        [InlineData(null)]
        [InlineData("")]
        [InlineData("    ")]
        public static void Should_not_allow_blank_title(string? title)
        {
            var testCmd = new CreateShoppingListCommand()
            {
                Title = title
            };

            Assert.False(testCmd.Validate(out var errors));
            Assert.Single(errors, new Error(ShoppingListErrors.BlankTitle));
        }

        [Fact]
        public static void Should_allow_valid_command()
        {
            var testCmd = new CreateShoppingListCommand()
            {
                Title = "Minha lista de compras"
            };

            Assert.True(testCmd.Validate(out var errors));
            Assert.Null(errors);
        }
    }
}