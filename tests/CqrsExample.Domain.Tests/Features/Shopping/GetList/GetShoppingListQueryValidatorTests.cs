using CqrsExample.Domain.Features.Shopping;
using CqrsExample.Domain.Features.Shopping.GetList;
using Xunit;

namespace CqrsExample.Domain.Tests.Features.Shopping.GetList;

public static class GetShoppingListQueryValidatorTests
{
    [Fact]
    public static void Should_not_allow_null_id()
    {
        var testQuery = new GetShoppingListQuery(null);

        Assert.False(testQuery.Validate(out var error));
        Assert.Equal(new(ShoppingListErrors.InvalidId), error!);
    }

    [Fact]
    public static void Should_not_allow_empty_id()
    {
        var testQuery = new GetShoppingListQuery(Guid.Empty);

        Assert.False(testQuery.Validate(out var error));
        Assert.Equal(new(ShoppingListErrors.InvalidId), error!);
    }

    [Fact]
    public static void Should_allow_valid_query()
    {
        var testQuery = new GetShoppingListQuery(Guid.NewGuid());

        Assert.True(testQuery.Validate(out var errors));
        Assert.Null(errors);
    }
}