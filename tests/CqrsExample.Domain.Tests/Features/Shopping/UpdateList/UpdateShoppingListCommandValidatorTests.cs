using CqrsExample.Domain.Features.Shopping;
using CqrsExample.Domain.Features.Shopping.Common.Entities;
using CqrsExample.Domain.Features.Shopping.UpdateList;
using Xunit;

namespace CqrsExample.Domain.Tests.Features.Shopping.UpdateList;

public static class UpdateShoppingListCommandValidatorTests
{
    [Fact]
    public static void Should_not_allow_null_id()
    {
        var testCmd = CreateValidTestCmd() with { Id = null };

        Assert.False(testCmd.Validate(out var error));
        Assert.Equal(new(ShoppingListErrors.InvalidId), error!);
    }

    [Fact]
    public static void Should_not_allow_empty_id()
    {
        var testCmd = CreateValidTestCmd() with { Id = Guid.Empty };

        Assert.False(testCmd.Validate(out var error));
        Assert.Equal(new(ShoppingListErrors.InvalidId), error!);
    }

    [Theory]
    [InlineData(null)]
    [InlineData("")]
    [InlineData("    ")]
    public static void Should_not_allow_blank_title(string? title)
    {
        var testCmd = CreateValidTestCmd() with { Title = title };

        Assert.False(testCmd.Validate(out var error));
        Assert.Equal(new(ShoppingListErrors.BlankTitle), error!);
    }

    [Fact]
    public static void Should_not_allow_items_array_null()
    {
        var testCmd = CreateValidTestCmd() with { Items = null };

        Assert.False(testCmd.Validate(out var error));
        Assert.Equal(new(ShoppingListErrors.ItemsNull), error!);
    }

    [Fact]
    public static void Should_not_allow_items_without_names()
    {
        var testCmd = CreateValidTestCmd([new(1, null!)]);

        Assert.False(testCmd.Validate(out var error));
        Assert.Equal(new(ShoppingListErrors.BlankItemName), error!);
    }

    [Fact]
    public static void Should_not_allow_items_with_quantity_equal_or_less_than_zero()
    {
        var testCmd = CreateValidTestCmd(
        [
            new(Quantity: 0, ItemName: "Arroz 5kg"),
            new(Quantity: 0, ItemName: "Feijão 1kg")
        ]);

        Assert.False(testCmd.Validate(out var error));
        Assert.NotNull(error);
        Assert.Equal(ShoppingListErrors.ItemQuantityZeroOrLess.Item1, error.Code);
        Assert.Equal("The item quantity must be greater than zero (item name: 'Arroz 5kg').",
                     error.Message);
    }

    [Fact]
    public static void Should_not_allow_repeated_items()
    {
        var testCmd = CreateValidTestCmd(
        [
            new(Quantity: 2, ItemName: "Arroz 5kg"),
            new(Quantity: 5, ItemName: "Arroz 5kg")
        ]);

        Assert.False(testCmd.Validate(out var error));
        Assert.Equal(new(ShoppingListErrors.RepeatedItems), error!);
    }

    [Fact]
    public static void Should_allow_valid_command()
    {
        var testCmd = CreateValidTestCmd();
        Assert.True(testCmd.Validate(out var error));
        Assert.Null(error);
    }

    private static UpdateShoppingListCommand CreateValidTestCmd(ShoppingListItem[]? items = null) => new(
        Id: Guid.NewGuid(),
        Title: "Minha lista de compras",
        Items: items ?? Array.Empty<ShoppingListItem>()
    );
}