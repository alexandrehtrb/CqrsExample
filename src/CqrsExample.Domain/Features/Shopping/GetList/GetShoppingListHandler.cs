using System.Text.Json.Serialization;
using CqrsExample.Domain.BaseAbstractions;
using CqrsExample.Domain.Features.Shopping.Common.Entities;
using CqrsExample.Domain.Features.Shopping.Common.Repositories;

namespace CqrsExample.Domain.Features.Shopping.GetList;

public sealed record GetShoppingListQuery(Guid? Id) : Query;

public sealed record GetShoppingListResult(
    Guid Id,
    string Title,
    ShoppingListItem[] Items,
    QueryResultFailureType? FailureType = null,
    CqrsError? Error = null) : QueryResult(FailureType, Error)
{
    private static GetShoppingListResult Failed(QueryResultFailureType failureType, CqrsError error) => new(
        Guid.Empty,
        string.Empty,
        [],
        failureType,
        error);

    public static readonly GetShoppingListResult InternalError =
        Failed(QueryResultFailureType.InternalError, new(ShoppingListErrors.InternalError));

    public static readonly GetShoppingListResult NotFound =
        Failed(QueryResultFailureType.NotFound, new(ShoppingListErrors.ShoppingListNotFound));

    public static GetShoppingListResult ValidationError(CqrsError error) =>
        Failed(QueryResultFailureType.Validation, error);

    public static GetShoppingListResult Success(ShoppingList list) => new(
        Id: list.Id,
        Title: list.Title,
        Items: list.Items
    );
}

public sealed class GetShoppingListHandler(IShoppingListReadRepository repo)
{
    public async Task<GetShoppingListResult> HandleAsync(GetShoppingListQuery qry)
    {
        if (!qry.Validate(out var error))
            return GetShoppingListResult.ValidationError(error!);

        var qryResult = await repo.GetAsync((Guid)qry.Id!);

        return qryResult != null ?
            GetShoppingListResult.Success(qryResult) :
            GetShoppingListResult.NotFound;
    }
}