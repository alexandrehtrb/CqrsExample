using System.Net;
using CqrsExample.Domain.BaseAbstractions;
using CqrsExample.Domain.Features.Shopping.CreateList;
using CqrsExample.Domain.Features.Shopping.GetList;
using CqrsExample.Domain.Features.Shopping.UpdateList;
using Microsoft.AspNetCore.Mvc;
#if !NATIVEAOT
using CqrsExample.Api.Configurations;
using CqrsExample.Domain.Features.Shopping;
using Microsoft.OpenApi;
using Scalar.AspNetCore;
using static CqrsExample.Api.Configurations.OpenApiConfiguration;
#endif

namespace CqrsExample.Api;

public static class Endpoints
{
    internal const string ContentTypeJson = "application/json; charset=utf-8";
    internal const string ContentTypeProblemDetailsJson = "application/problem+json; charset=utf-8";

    public static WebApplication MapAllEndpoints(this WebApplication app)
    {
        app.MapGet("/{id:Guid}", GetShoppingListAsync)
#if !NATIVEAOT
           .WithName(nameof(GetShoppingListAsync)) // required for examples
           .WithTags("tag1")
           .WithSummary("Retrieves a shopping list.")
           .WithDescription("### markdown subheader\n\ntext here\n\n```powershell\necho 'Hello!';\n```")
           .Produces<GetShoppingListResult>((int)HttpStatusCode.OK, ContentTypeJson)
           .ProducesProblem((int)HttpStatusCode.BadRequest, ContentTypeProblemDetailsJson)
           .ProducesProblem((int)HttpStatusCode.NotFound, ContentTypeProblemDetailsJson)
           .ProducesProblem((int)HttpStatusCode.InternalServerError, ContentTypeProblemDetailsJson)
#endif
           ;

        app.MapPost("/", CreateShoppingListAsync)
#if !NATIVEAOT
           .WithName(nameof(CreateShoppingListAsync)) // required for examples
           .WithTags("tag1")
           .WithSummary("Creates a new shopping list.")
           .WithDescription("### markdown subheader\n\ntext here\n\n```js\nconsole.log('Hello!');\n```")
           .Accepts<CreateShoppingListCommand>(ContentTypeJson)
           .Produces<CreateShoppingListResult>((int)HttpStatusCode.Created, ContentTypeJson)
           .ProducesProblem((int)HttpStatusCode.BadRequest, ContentTypeProblemDetailsJson)
           .ProducesProblem((int)HttpStatusCode.InternalServerError, ContentTypeProblemDetailsJson)
#endif
           ;

        app.MapPut("/{id:Guid}", UpdateShoppingListAsync)
#if !NATIVEAOT
           .WithName(nameof(UpdateShoppingListAsync)) // required for examples
           .WithTags("tag1")
           .WithSummary("Updates a shopping list.")
           .WithDescription("### markdown subheader\n\ntext here\n\n```cs\nConsole.WriteLine(\"Hello!\");\n```")
           .Accepts<UpdateShoppingListCommand>(ContentTypeJson)
           .Produces((int)HttpStatusCode.NoContent)
           .ProducesProblem((int)HttpStatusCode.BadRequest, ContentTypeProblemDetailsJson)
           .ProducesProblem((int)HttpStatusCode.NotFound, ContentTypeProblemDetailsJson)
           .ProducesProblem((int)HttpStatusCode.InternalServerError, ContentTypeProblemDetailsJson)
#endif
           ;

#if !NATIVEAOT
        app.MapOpenApi();
        app.MapScalarApiReference(sco => sco.DefaultHttpClient = new(ScalarTarget.Http, ScalarClient.Http11));
#endif

        return app;
    }

    private static async Task<IResult> GetShoppingListAsync(
        [FromServices] ILogger<GetShoppingListHandler> logger,
        [FromServices] GetShoppingListHandler handler,
        [FromRoute] Guid id)
    {
        GetShoppingListQuery qry = new(id!);
        var qryResult = await handler.HandleAsync(qry);
        logger.LogQuery("Get shopping list", "Failed to get shopping list", qry, qryResult);
        return ResponseForQueryResult(qryResult);
    }

    public static async Task<IResult> CreateShoppingListAsync(
        [FromServices] ILogger<CreateShoppingListHandler> logger,
        [FromServices] CreateShoppingListHandler handler,
        [FromBody] CreateShoppingListCommand cmd)
    {
        var cmdResult = await handler.HandleAsync(cmd);
        // not using ResponseForCommandResult here because we want to return an object
        logger.LogCommand("Create shopping list", "Failed to create shopping list", cmd, cmdResult);
        return cmdResult.Successful ? Results.Created(cmdResult.Id.ToString(), cmdResult) : Problem(cmdResult);
    }

    private static async Task<IResult> UpdateShoppingListAsync(
        [FromServices] ILogger<UpdateShoppingListHandler> logger,
        [FromServices] UpdateShoppingListHandler handler,
        [FromRoute] Guid id,
        [FromBody] UpdateShoppingListCommand cmd)
    {
        cmd = cmd with { Id = id };
        var cmdResult = await handler.HandleAsync(cmd);
        logger.LogCommand("Update shopping list", "Failed to update shopping list", cmd, cmdResult);
        return ResponseForCommandResult(cmdResult);
    }

#if !NATIVEAOT   

    #region REQUEST AND RESPONSE EXAMPLES

    public static Task DescribeGetShoppingListExamples(OpenApiOperation op)
    {
        if (op.OperationId == nameof(GetShoppingListAsync))
        {
            op.AddResponseExample<GetShoppingListResult>(
                HttpStatusCode.OK,
                new(Guid.NewGuid(), "My shopping list", [new(1, "Rice 5kg"), new(2, "Beans 1kg")]),
                ApiJsonSrcGenContext.Default.GetShoppingListResult);

            op.AddProblemDetailsResponseExamples(
                HttpStatusCode.BadRequest,
                new()
                {
                    { nameof(ShoppingListErrors.InvalidId), ShoppingListErrors.InvalidId }
                });

            op.AddProblemDetailsResponseExamples(
                HttpStatusCode.NotFound,
                new()
                {
                    { nameof(ShoppingListErrors.ShoppingListNotFound), ShoppingListErrors.ShoppingListNotFound }
                });

            op.AddProblemDetailsResponseExamples(
                HttpStatusCode.InternalServerError,
                new()
                {
                    { nameof(ShoppingListErrors.InternalError), ShoppingListErrors.InternalError }
                });
        }

        return Task.CompletedTask;
    }

    public static Task DescribeCreateShoppingListExamples(OpenApiOperation op)
    {
        if (op.OperationId == nameof(CreateShoppingListAsync))
        {
            op.AddRequestExample<CreateShoppingListCommand>(
                new("My shopping list"),
                ApiJsonSrcGenContext.Default.CreateShoppingListCommand);

            op.AddResponseExample<CreateShoppingListResult>(
                HttpStatusCode.Created,
                new(Guid.NewGuid(), "My shopping list", []),
                ApiJsonSrcGenContext.Default.CreateShoppingListResult);

            op.AddProblemDetailsResponseExamples(
                HttpStatusCode.BadRequest,
                new()
                {
                    { nameof(ShoppingListErrors.BlankTitle), ShoppingListErrors.BlankTitle }
                });

            op.AddProblemDetailsResponseExamples(
                HttpStatusCode.InternalServerError,
                new()
                {
                    { nameof(ShoppingListErrors.InternalError), ShoppingListErrors.InternalError }
                });
        }

        return Task.CompletedTask;
    }

    public static Task DescribeUpdateShoppingListExamples(OpenApiOperation op)
    {
        if (op.OperationId == nameof(UpdateShoppingListAsync))
        {
            op.AddRequestExample<UpdateShoppingListCommand>(
                new(Guid.NewGuid(), "My shopping list", [new(1, "Rice 5kg"), new(2, "Beans 1kg")]),
                ApiJsonSrcGenContext.Default.UpdateShoppingListCommand);

            op.AddProblemDetailsResponseExamples(
                HttpStatusCode.BadRequest,
                new()
                {
                    { nameof(ShoppingListErrors.InvalidId), ShoppingListErrors.InvalidId },
                    { nameof(ShoppingListErrors.BlankTitle), ShoppingListErrors.BlankTitle },
                    { nameof(ShoppingListErrors.ItemsNull), ShoppingListErrors.ItemsNull },
                    { nameof(ShoppingListErrors.BlankItemName), ShoppingListErrors.BlankItemName },
                    { nameof(ShoppingListErrors.ItemQuantityZeroOrLess), ShoppingListErrors.ItemQuantityZeroOrLess },
                    { nameof(ShoppingListErrors.RepeatedItems), ShoppingListErrors.RepeatedItems }
                });

            op.AddProblemDetailsResponseExamples(
                HttpStatusCode.NotFound,
                new()
                {
                    { nameof(ShoppingListErrors.ShoppingListNotFound), ShoppingListErrors.ShoppingListNotFound }
                });

            op.AddProblemDetailsResponseExamples(
                HttpStatusCode.InternalServerError,
                new()
                {
                    { nameof(ShoppingListErrors.InternalError), ShoppingListErrors.InternalError }
                });
        }

        return Task.CompletedTask;
    }

    #endregion

#endif

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

    private static void LogCommand<T, C, R>(this ILogger<T> logger, string successMsg, string failMsg, C cmd, R cmdResult)
        where C : Command
        where R : CommandResult
    {
        using (logger.BeginScope("{@cmd}", cmd))
        {
            if (cmdResult.Successful)
            {
                using (logger.BeginScope("{@cmdResult}", cmdResult))
                {
                    logger.LogInformation(successMsg);
                }
            }
            else
            {
                using (logger.BeginScope("{@cmdResultFailureType} {@cmdResultError}", cmdResult.FailureType, cmdResult.Error))
                {
                    logger.LogWarning(failMsg);
                }
            }
        }
    }

    private static void LogQuery<T, Q, R>(this ILogger<T> logger, string successMsg, string failMsg, Q qry, R qryResult)
        where Q : Query
        where R : QueryResult
    {
        using (logger.BeginScope("{@qry}", qry))
        {
            if (qryResult.Successful)
            {
                using (logger.BeginScope("{@qryResult}", qryResult))
                {
                    logger.LogInformation(successMsg);
                }
            }
            else
            {
                using (logger.BeginScope("{@qryResultFailureType} {@qryResultError}", qryResult.FailureType, qryResult.Error))
                {
                    logger.LogWarning(failMsg);
                }
            }
        }
    }

    #endregion
}
