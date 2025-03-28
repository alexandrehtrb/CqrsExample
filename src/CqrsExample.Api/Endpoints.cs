using System.Net;
using CqrsExample.Domain.BaseAbstractions;
using CqrsExample.Domain.Features.Shopping.CreateList;
using CqrsExample.Domain.Features.Shopping.GetList;
using CqrsExample.Domain.Features.Shopping.UpdateList;
using Microsoft.AspNetCore.Mvc;
using Scalar.AspNetCore;

namespace CqrsExample.Api;

public static class Endpoints
{
    private const string contentTypeJson = "application/json; charset=utf-8";

    public static WebApplication MapAllEndpoints(this WebApplication app)
    {
        app.MapGet("/{id:Guid}", GetShoppingListAsync)
            .WithSummary("Retrieves a shopping list.")
            .WithTags("tag1")
            .Produces<GetShoppingListResult>((int)HttpStatusCode.OK, contentTypeJson)
            .ProducesProblem((int)HttpStatusCode.BadRequest, contentTypeJson)
            .ProducesProblem((int)HttpStatusCode.NotFound, contentTypeJson)
            .ProducesProblem((int)HttpStatusCode.InternalServerError, contentTypeJson);

        app.MapPost("/", CreateShoppingListAsync)
           .WithSummary("Creates a new shopping list.")
           .WithTags("tag1")
           .Accepts<CreateShoppingListCommand>(contentTypeJson)
           .Produces<CreateShoppingListResult>((int)HttpStatusCode.Created, contentTypeJson)
           .ProducesProblem((int)HttpStatusCode.BadRequest, contentTypeJson)
           .ProducesProblem((int)HttpStatusCode.NotFound, contentTypeJson)
           .ProducesProblem((int)HttpStatusCode.InternalServerError, contentTypeJson);

        app.MapPut("/{id:Guid}", UpdateShoppingListAsync)
           .WithSummary("Updates a shopping list.")
           .WithTags("tag1")
           .Accepts<UpdateShoppingListCommand>(contentTypeJson)
           .Produces((int)HttpStatusCode.NoContent)
           .ProducesProblem((int)HttpStatusCode.BadRequest, contentTypeJson)
           .ProducesProblem((int)HttpStatusCode.NotFound, contentTypeJson)
           .ProducesProblem((int)HttpStatusCode.InternalServerError, contentTypeJson);

        if (app.Environment.IsProduction() == false)
        {
            app.MapOpenApi();
            app.MapScalarApiReference();
        }

        return app;
    }

    private static async Task<IResult> GetShoppingListAsync(
        [FromServices] GetShoppingListHandler handler,
        [FromRoute] Guid id)
    {
        GetShoppingListQuery qry = new(id!);
        var qryResult = await handler.HandleAsync(qry);
        return ResponseForQueryResult(qryResult);
    }

    private static async Task<IResult> CreateShoppingListAsync(
        [FromServices] CreateShoppingListHandler handler,
        [FromBody] CreateShoppingListCommand cmd)
    {
        var cmdResult = await handler.HandleAsync(cmd);
        // not using ResponseForCommandResult here because we want to return an object
        return cmdResult.Successful ? Results.Created(cmdResult.Id.ToString(), cmdResult) : Problem(cmdResult);
    }

    private static async Task<IResult> UpdateShoppingListAsync(
        [FromServices] UpdateShoppingListHandler handler,
        [FromRoute] Guid id,
        [FromBody] UpdateShoppingListCommand cmd)
    {
        cmd = cmd with { Id = id };
        var cmdResult = await handler.HandleAsync(cmd);
        return ResponseForCommandResult(cmdResult);
    }

    #region HELPERS

    private static IResult Problem(CommandResult cmdResult)
    {
        var statusCode = cmdResult.FailureType switch
        {
            CommandResultFailureType.Unauthorized => HttpStatusCode.Unauthorized,
            CommandResultFailureType.Forbidden => HttpStatusCode.Forbidden,
            CommandResultFailureType.Validation => HttpStatusCode.BadRequest,
            CommandResultFailureType.NotFound => HttpStatusCode.NotFound,
            CommandResultFailureType.Unprocessable => HttpStatusCode.UnprocessableEntity,
            _ => HttpStatusCode.InternalServerError
        };

        return Results.Problem(statusCode: (int)statusCode,
                               type: Enum.GetName((CommandResultFailureType)cmdResult.FailureType!),
                               title: cmdResult.Error!.Code,
                               detail: cmdResult.Error!.Message);
    }

    private static IResult Problem(QueryResult qryResult)
    {
        // This isn't a "failure"
        if (qryResult.FailureType == QueryResultFailureType.Unmodified)
        {
            return Results.StatusCode((int)HttpStatusCode.NotModified);
        }

        var statusCode = qryResult.FailureType switch
        {
            QueryResultFailureType.Unauthorized => HttpStatusCode.Unauthorized,
            QueryResultFailureType.Forbidden => HttpStatusCode.Forbidden,
            QueryResultFailureType.Validation => HttpStatusCode.BadRequest,
            QueryResultFailureType.NotFound => HttpStatusCode.NotFound,
            _ => HttpStatusCode.InternalServerError
        };

        return Results.Problem(statusCode: (int)statusCode,
                               type: Enum.GetName((QueryResultFailureType)qryResult.FailureType!),
                               title: qryResult.Error!.Code,
                               detail: qryResult.Error!.Message);
    }

    // Important: ideally, commands should not return values (CQRS),
    // but there are some cases that it makes sense for them to return,
    // like login user and renew access token.
    // In those cases, simply copy the ternary line below into the method,
    // replacing Results.NoContent() for Results.Ok(cmdResult).
    private static IResult ResponseForCommandResult<T>(T cmdResult) where T : CommandResult =>
        cmdResult.Successful ? Results.NoContent() : Problem(cmdResult);

    private static IResult ResponseForQueryResult<T>(T qryResult) where T : QueryResult =>
        qryResult.Successful ? Results.Ok(qryResult) : Problem(qryResult);

    #endregion
}
