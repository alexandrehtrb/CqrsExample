using System;
using System.Net;
using System.Threading.Tasks;
using CqrsExample.Api.Configurations;
using CqrsExample.Api.Features.Shopping.RequestExamples.CreateList;
using CqrsExample.Api.Features.Shopping.RequestExamples.UpdateList;
using CqrsExample.Api.Features.Shopping.ResponseExamples.GetList;
using CqrsExample.Api.Features.Shopping.ResponseExamples.CreateList;
using CqrsExample.Api.Features.Shopping.ResponseExamples.UpdateList;
using CqrsExample.Domain.BaseAbstractions.Commands;
using CqrsExample.Domain.BaseAbstractions.Errors;
using CqrsExample.Domain.BaseAbstractions.Queries;
using CqrsExample.Domain.Features.Shopping.GetList;
using CqrsExample.Domain.Features.Shopping.CreateList;
using CqrsExample.Domain.Features.Shopping.UpdateList;
using Microsoft.AspNetCore.Mvc;
using Swashbuckle.AspNetCore.Annotations;
using Swashbuckle.AspNetCore.Filters;

namespace CqrsExample.Api.Features.Shopping
{
    [ApiController]
    [Route("shoppinglist")]
    public class ShoppingListController : ControllerBase
    {
        private readonly ICommandHandler<CreateShoppingListCommand, CreateShoppingListResult> createShoppingListHandler;
        private readonly ICommandHandler<UpdateShoppingListCommand, UpdateShoppingListResult> updateShoppingListHandler;
        private readonly IQueryHandler<GetShoppingListQuery, GetShoppingListResult> getShoppingListHandler;

        public ShoppingListController(
            ICommandHandler<CreateShoppingListCommand, CreateShoppingListResult> createShoppingListHandler,
            ICommandHandler<UpdateShoppingListCommand, UpdateShoppingListResult> updateShoppingListHandler,
            IQueryHandler<GetShoppingListQuery, GetShoppingListResult> getShoppingListHandler)
        {
            this.createShoppingListHandler = createShoppingListHandler;
            this.updateShoppingListHandler = updateShoppingListHandler;
            this.getShoppingListHandler = getShoppingListHandler;
        }

        /// <summary>Creates a new shopping list.</summary>
        /// <remarks>Creates a new shopping list according to the information passed.</remarks>
        /// <param name="cmd">Information to create the shopping list.</param>
        /// <response code="201">The created shopping list.</response>
        /// <response code="400">When the request is not valid.</response>
        /// <response code="500">If an internal error occurred.</response>
        [ProducesResponseType(typeof(CreateShoppingListResult), (int) HttpStatusCode.Created)]
        [ProducesResponseType(typeof(Error[]), (int) HttpStatusCode.BadRequest)]
        [ProducesResponseType(typeof(Error[]), (int) HttpStatusCode.InternalServerError)]
        [SwaggerRequestExample(typeof(CreateShoppingListCommand), typeof(CreateShoppingListRequestExample))]
        [SwaggerResponse((int) HttpStatusCode.Created, Type = typeof(CreateShoppingListResult))]
        [SwaggerResponseExample((int) HttpStatusCode.Created, typeof(CreateShoppingListSuccessExample))]
        [SwaggerResponse((int) HttpStatusCode.BadRequest, Type = typeof(Error[]))]
        [SwaggerResponseExample((int) HttpStatusCode.BadRequest, typeof(CreateShoppingListValidationErrorsExample))]
        [SwaggerResponse((int) HttpStatusCode.InternalServerError, Type = typeof(Error[]))]
        [SwaggerResponseExample((int) HttpStatusCode.InternalServerError, typeof(CreateShoppingListInternalErrorsExample))]
        [Produces("application/json")]
        [Consumes("application/json")]
        [HttpPost]
        public async Task<IActionResult> CreateShoppingList([FromBody] CreateShoppingListCommand cmd)
        {
            var result = await createShoppingListHandler.HandleAsync(cmd);

            if (result.IsSuccess)
                return CreatedAtAction(nameof(GetShoppingList), new { id = result.SuccessResult!.Id }, result.SuccessResult);
            else if (result.FailureType == CommandResultFailureType.Validation)
                return BadRequest(result.Errors);
            else
                return this.InternalServerError(result.Errors!);
        }

        /// <summary>Updates a shopping list.</summary>
        /// <remarks>Updates a shopping list, changing its items and title.</remarks>
        /// <param name="id">The shopping list id.</param>
        /// <param name="cmd">Information to update the shopping list.</param>
        /// <response code="204">If the shopping list was updated.</response>
        /// <response code="400">When the request is not valid.</response>
        /// <response code="404">When the shopping list could not be found.</response>
        /// <response code="500">If an internal error occurred.</response>
        [ProducesResponseType(typeof(UpdateShoppingListResult), (int) HttpStatusCode.NoContent)]
        [ProducesResponseType(typeof(Error[]), (int) HttpStatusCode.BadRequest)]
        [ProducesResponseType(typeof(Error[]), (int) HttpStatusCode.NotFound)]
        [ProducesResponseType(typeof(Error[]), (int) HttpStatusCode.InternalServerError)]
        [SwaggerRequestExample(typeof(UpdateShoppingListCommand), typeof(UpdateShoppingListRequestExample))]
        [SwaggerResponse((int) HttpStatusCode.NoContent)]
        [SwaggerResponse((int) HttpStatusCode.BadRequest, Type = typeof(Error[]))]
        [SwaggerResponseExample((int) HttpStatusCode.BadRequest, typeof(UpdateShoppingListValidationErrorsExample))]
        [SwaggerResponse((int) HttpStatusCode.NotFound, Type = typeof(Error[]))]
        [SwaggerResponseExample((int) HttpStatusCode.NotFound, typeof(UpdateShoppingListNotFoundErrorsExample))]
        [SwaggerResponse((int) HttpStatusCode.InternalServerError, Type = typeof(Error[]))]
        [SwaggerResponseExample((int) HttpStatusCode.InternalServerError, typeof(UpdateShoppingListInternalErrorsExample))]
        [Produces("application/json")]
        [Consumes("application/json")]
        [HttpPut("{id}")]
        public async Task<IActionResult> UpdateShoppingList(Guid? id, [FromBody] UpdateShoppingListCommand cmd)
        {
            cmd.SetId(id);
            var result = await updateShoppingListHandler.HandleAsync(cmd);

            if (result.IsSuccess)
                return NoContent();
            else if (result.FailureType == CommandResultFailureType.Validation)
                return BadRequest(result.Errors);
            else if (result.FailureType == CommandResultFailureType.NotFound)
                return NotFound(result.Errors);                
            else
                return this.InternalServerError(result.Errors!);
        }

        /// <summary>Retrieves a shopping list.</summary>
        /// <remarks>Retrieves a shopping list for a given id.</remarks>
        /// <param name="id">The shopping list id.</param>
        /// <response code="200">The shopping list.</response>
        /// <response code="400">When the shopping list id is not valid.</response>
        /// <response code="404">When the shopping list could not be found.</response>
        /// <response code="500">If an internal error occurred.</response>
        [ProducesResponseType(typeof(GetShoppingListResult), (int) HttpStatusCode.OK)]
        [ProducesResponseType(typeof(Error[]), (int) HttpStatusCode.BadRequest)]
        [ProducesResponseType(typeof(Error[]), (int) HttpStatusCode.NotFound)]
        [ProducesResponseType(typeof(Error[]), (int) HttpStatusCode.InternalServerError)]
        [SwaggerResponse((int) HttpStatusCode.OK, Type = typeof(GetShoppingListResult))]
        [SwaggerResponseExample((int) HttpStatusCode.OK, typeof(GetShoppingListSuccessExample))]
        [SwaggerResponse((int) HttpStatusCode.BadRequest, Type = typeof(Error[]))]
        [SwaggerResponseExample((int) HttpStatusCode.BadRequest, typeof(GetShoppingListValidationErrorsExample))]
        [SwaggerResponse((int) HttpStatusCode.NotFound, Type = typeof(Error[]))]
        [SwaggerResponseExample((int) HttpStatusCode.NotFound, typeof(GetShoppingListNotFoundErrorsExample))]
        [SwaggerResponse((int) HttpStatusCode.InternalServerError, Type = typeof(Error[]))]
        [SwaggerResponseExample((int) HttpStatusCode.InternalServerError, typeof(GetShoppingListInternalErrorsExample))]
        [Produces("application/json")]
        [Consumes("application/json")]
        [HttpGet("{id}", Name = nameof(GetShoppingList))]
        public async Task<IActionResult> GetShoppingList(Guid? id)
        {
            var qry = new GetShoppingListQuery(id);
            var result = await getShoppingListHandler.HandleAsync(qry);

            if (result.IsSuccess)
                return Ok(result.SuccessResult);
            else if (result.FailureType == QueryResultFailureType.Validation)
                return BadRequest(result.Errors);
            else if (result.FailureType == QueryResultFailureType.NotFound)
                return NotFound(result.Errors);                
            else
                return this.InternalServerError(result.Errors!);
        }
    }
}