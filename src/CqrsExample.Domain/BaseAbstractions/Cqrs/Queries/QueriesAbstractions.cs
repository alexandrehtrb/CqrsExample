using System.Threading.Tasks;
using CqrsExample.Domain.BaseAbstractions.Errors;

namespace CqrsExample.Domain.BaseAbstractions.Queries;

public abstract class Query
{
}

public enum QueryResultFailureType
{
    Validation = 1,
    NotFound = 2,
    InternalError = 3
}

public abstract class QueryResult
{
}

public sealed class QueryResult<A>
    where A : QueryResult
{
    public A? SuccessResult { get; private set; }
    public QueryResultFailureType FailureType { get; private set; }
    public Error[]? Errors { get; private set; }
    public bool IsSuccess => Errors == null || Errors.Length == 0;

    private QueryResult(A successResult) => SuccessResult = successResult;
    private QueryResult(QueryResultFailureType failureType, Error[] errors)
    {
        FailureType = failureType;
        Errors = errors;
    }

    public static QueryResult<A> Success(A successResult) => new(successResult);
    public static QueryResult<A> Fail(QueryResultFailureType failureType, Error[] errors) => new(failureType, errors);
    public static QueryResult<A> Fail(QueryResultFailureType failureType, Error error) => Fail(failureType, new[] { error });
    public static QueryResult<A> NotFound(Error notFoundError) => Fail(QueryResultFailureType.NotFound, notFoundError);
}

public interface IQueryHandler<Q, R>
    where Q : Query
    where R : QueryResult
{
    Task<QueryResult<R>> HandleAsync(Q qry);
}