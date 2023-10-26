using System;
using CqrsExample.Domain.BaseAbstractions.Errors;
using CqrsExample.Domain.Features.Shopping;
using CqrsExample.Domain.Features.Shopping.GetList;
using Xunit;

namespace CqrsExample.Domain.Tests.Feature.Shopping.GetList;

public static class GetShoppingListQueryValidatorTests
{
    [Fact]
    public static void Should_not_allow_null_id()
    {
        var testQuery = new GetShoppingListQuery(null);

        Assert.False(testQuery.Validate(out var errors));
        Assert.Single(errors!, new Error(ShoppingListErrors.InvalidId));
    }

    [Fact]
    public static void Should_not_allow_empty_id()
    {
        var testQuery = new GetShoppingListQuery(Guid.Empty);

        Assert.False(testQuery.Validate(out var errors));
        Assert.Single(errors!, new Error(ShoppingListErrors.InvalidId));
    }

    [Fact]
    public static void Should_allow_valid_query()
    {
        var testQuery = new GetShoppingListQuery(Guid.NewGuid());

        Assert.True(testQuery.Validate(out var errors));
        Assert.Null(errors);
    }
}