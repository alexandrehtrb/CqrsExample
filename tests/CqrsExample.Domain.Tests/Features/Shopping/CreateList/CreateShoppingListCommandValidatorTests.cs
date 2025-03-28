using CqrsExample.Domain.Features.Shopping;
using CqrsExample.Domain.Features.Shopping.CreateList;
using Xunit;

namespace CqrsExample.Domain.Tests.Features.Shopping.CreateList;

public static class CreateShoppingListCommandValidatorTests
{
    [Theory]
    [InlineData(null)]
    [InlineData("")]
    [InlineData("    ")]
    public static void Should_not_allow_blank_title(string? title)
    {
        var testCmd = new CreateShoppingListCommand(title);

        Assert.False(testCmd.Validate(out var error));
        Assert.Equal(new(ShoppingListErrors.BlankTitle), error!);
    }

    [Fact]
    public static void Should_allow_valid_command()
    {
        var testCmd = new CreateShoppingListCommand("Minha lista de compras");

        Assert.True(testCmd.Validate(out var error));
        Assert.Null(error);
    }
}