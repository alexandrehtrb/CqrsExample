using System.Net;
using CqrsExample.Api.Configurations;
using CqrsExample.Domain.BaseAbstractions;
using CqrsExample.Domain.Features.Shopping;
using CqrsExample.Domain.Features.Shopping.Common.Entities;
using CqrsExample.Domain.Features.Shopping.CreateList;
using CqrsExample.Domain.Features.Shopping.GetList;
using CqrsExample.Domain.Features.Shopping.UpdateList;
using Microsoft.AspNetCore.Mvc;
#if !PRODUCTION
using Microsoft.AspNetCore.OpenApi;
using Microsoft.OpenApi.Any;
using Microsoft.OpenApi.Models;
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
#if !PRODUCTION
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
#if !PRODUCTION
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
#if !PRODUCTION
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

#if !PRODUCTION
        app.MapOpenApi();
        app.MapScalarApiReference(sco => sco.DefaultHttpClient = new(ScalarTarget.Http, ScalarClient.Http11));
#endif

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

    public static async Task<IResult> CreateShoppingListAsync(
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

#if !PRODUCTION   

    #region REQUEST AND RESPONSE EXAMPLES

    public static Task DescribeGetShoppingListExamples(OpenApiOperation op)
    {
        if (op.OperationId == nameof(GetShoppingListAsync))
        {
            op.AddResponseExample<GetShoppingListResult>(
                HttpStatusCode.OK,
                new(Guid.NewGuid(), "My shopping list", [new(1, "Rice 5kg"), new(2, "Beans 1kg")]),
                JsonConfiguration.JsonCtx.GetShoppingListResult);

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
                JsonConfiguration.JsonCtx.CreateShoppingListCommand);

            op.AddResponseExample<CreateShoppingListResult>(
                HttpStatusCode.Created,
                new(Guid.NewGuid(), "My shopping list", []),
                JsonConfiguration.JsonCtx.CreateShoppingListResult);

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
                JsonConfiguration.JsonCtx.UpdateShoppingListCommand);

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

    #endregion
}
