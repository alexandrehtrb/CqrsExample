using System;
using System.Threading.Tasks;
using CqrsExample.Domain.BaseAbstractions.Errors;
using CqrsExample.Domain.BaseAbstractions.Queries;
using CqrsExample.Domain.Features.Shopping.Common.Repositories;

namespace CqrsExample.Domain.Features.Shopping.GetList;

public sealed class GetShoppingListHandler : IQueryHandler<GetShoppingListQuery, GetShoppingListResult>
{
    private static readonly Error notFoundError = new(ShoppingListErrors.ShoppingListNotFound);

    private readonly IShoppingListReadRepository repository;

    public GetShoppingListHandler(IShoppingListReadRepository repository) =>
        this.repository = repository;

    public async Task<QueryResult<GetShoppingListResult>> HandleAsync(GetShoppingListQuery qry)
    {
        if (!qry.Validate(out var errors))
            return QueryResult<GetShoppingListResult>.Fail(QueryResultFailureType.Validation, errors!);

        var qryResult = await this.repository.QueryAsync<GetShoppingListResult>((Guid)qry.Id!);

        return qryResult != null ?
            QueryResult<GetShoppingListResult>.Success(qryResult) :
            QueryResult<GetShoppingListResult>.NotFound(notFoundError);
    }
}