using System;
using CqrsExample.Domain.BaseAbstractions.Errors;
using CqrsExample.Domain.Features.Shopping;
using CqrsExample.Domain.Features.Shopping.Common.Entities;
using CqrsExample.Domain.Features.Shopping.UpdateList;
using Xunit;

namespace CqrsExample.Domain.Tests.Feature.Shopping.GetList;

public static class UpdateShoppingListCommandValidatorTests
{
    [Fact]
    public static void Should_not_allow_null_id()
    {
        var testCmd = CreateValidTestCmd();
        testCmd.SetId(null);

        Assert.False(testCmd.Validate(out var errors));
        Assert.Single(errors!, new Error(ShoppingListErrors.InvalidId));
    }

    [Fact]
    public static void Should_not_allow_empty_id()
    {
        var testCmd = CreateValidTestCmd();
        testCmd.SetId(Guid.Empty);

        Assert.False(testCmd.Validate(out var errors));
        Assert.Single(errors!, new Error(ShoppingListErrors.InvalidId));
    }

    [Theory]
    [InlineData(null)]
    [InlineData("")]
    [InlineData("    ")]
    public static void Should_not_allow_blank_title(string? title)
    {
        var testCmd = CreateValidTestCmd();
        testCmd.Title = title;

        Assert.False(testCmd.Validate(out var errors));
        Assert.Single(errors!, new Error(ShoppingListErrors.BlankTitle));
    }

    [Fact]
    public static void Should_not_allow_items_array_null()
    {
        var testCmd = CreateValidTestCmd();
        testCmd.Items = null;

        Assert.False(testCmd.Validate(out var errors));
        Assert.Single(errors!, new Error(ShoppingListErrors.ItemsNull));
    }

    [Fact]
    public static void Should_not_allow_items_without_names()
    {
        var testCmd = CreateValidTestCmd();
        testCmd.Items = new[] { new ShoppingListItem() { Quantity = 1 } };

        Assert.False(testCmd.Validate(out var errors));
        Assert.Single(errors!, new Error(ShoppingListErrors.BlankItemName));
    }

    [Fact]
    public static void Should_not_allow_items_with_quantity_equal_or_less_than_zero()
    {
        var testCmd = CreateValidTestCmd();
        testCmd.Items = new[] {
            new ShoppingListItem(){ Quantity = 0, ItemName = "Arroz 5kg" },
            new ShoppingListItem(){ Quantity = 0, ItemName = "Feijão 1kg" }
        };

        Assert.False(testCmd.Validate(out var errors));
        Assert.NotNull(errors);
        Assert.Equal(2, errors!.Length);
        Assert.Equal(ShoppingListErrors.ItemQuantityZeroOrLess.Item1, errors[0].Code);
        Assert.Equal("The item quantity must be greater than zero (item name: Arroz 5kg).",
                     errors[0].Message);
        Assert.Equal(ShoppingListErrors.ItemQuantityZeroOrLess.Item1, errors[1].Code);
        Assert.Equal("The item quantity must be greater than zero (item name: Feijão 1kg).",
                     errors[1].Message);
    }

    [Fact]
    public static void Should_not_allow_repeated_items()
    {
        var testCmd = CreateValidTestCmd();
        testCmd.Items = new[] {
            new ShoppingListItem(){ Quantity = 2, ItemName = "Arroz 5kg" },
            new ShoppingListItem(){ Quantity = 5, ItemName = "Arroz 5kg" }
        };

        Assert.False(testCmd.Validate(out var errors));
        Assert.Single(errors!, new Error(ShoppingListErrors.RepeatedItems));
    }

    [Fact]
    public static void Should_allow_valid_command()
    {
        var testCmd = CreateValidTestCmd();

        Assert.True(testCmd.Validate(out var errors));
        Assert.Null(errors);
    }

    private static UpdateShoppingListCommand CreateValidTestCmd()
    {
        var cmd = new UpdateShoppingListCommand()
        {
            Title = "Minha lista de compras",
            Items = Array.Empty<ShoppingListItem>()
        };
        cmd.SetId(Guid.NewGuid());
        return cmd;
    }
}