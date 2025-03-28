using System.Text.Json.Serialization;

namespace CqrsExample.Domain.BaseAbstractions;

/*

|      CqrsError     |   ProblemDetails          |
|--------------------|---------------------------|
|       Code         |          Title            | (title deve ser uma constante)
|      Message       |          Detail           | (detail deve explicar com clareza para uma pessoa)
|       Status       |   CqrsResultFailureType   | (é o HTTP status code)
|        Type        |  (link da doc para erro)  | (link para documentação, a partir do Code)

*/

public sealed record CqrsError(string Code, string Message)
{
    public CqrsError((string Code, string Message) codeMessage, params object[] messageFormatArgs)
       : this(codeMessage.Code, string.Format(codeMessage.Message, messageFormatArgs))
    {
    }
}

public abstract record Command;

public enum CommandResultFailureType
{
    Unauthorized = 0,
    Forbidden = 1,
    Validation = 2,
    NotFound = 3,
    Unprocessable = 4,
    InternalError = 5
}

public abstract record CommandResult(
    [property: JsonIgnore] CommandResultFailureType? FailureType = null,
    [property: JsonIgnore] CqrsError? Error = null)
{
    [JsonIgnore]
    public bool Successful => Error == null;
}

public abstract record Query;

public abstract record CacheableQuery : Query
{
    public abstract string GetCacheKey();
}

public enum QueryResultFailureType
{
    Unauthorized = 0,
    Forbidden = 1,
    Validation = 2,
    NotFound = 3,
    Unmodified = 4,
    InternalError = 5
}

public abstract record QueryResult(
    [property: JsonIgnore] QueryResultFailureType? FailureType = null,
    [property: JsonIgnore] CqrsError? Error = null)
{
    [JsonIgnore]
    public bool Successful => Error == null;
}