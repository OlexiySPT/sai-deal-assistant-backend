using AutoMapper;
using MediatR;
using Microsoft.AspNetCore.Mvc;
using Sai.DealAssistant.Application;
using Sai.DealAssistant.Application.Entities.Deals.Commands;
using Sai.DealAssistant.Application.Entities.Deals.Queries;
using Sai.DealAssistant.Application.Entities.SampleCustomers.Dtos;
using Sai.DealAssistant.Application.Entities.SampleCustomers.Queries;
using System.Net.Mime;

namespace Sai.DealAssistant.WebApi.Controllers
{
    public class DealsController : BaseController
	{
		public DealsController(IMediator mediator, IMapper mapper)
			: base(mediator, mapper)
		{
		}

		/// <summary>
		/// Get list of Deals.
		/// </summary>
		/// <param name="query">Query parameters.</param>
		/// <response code="200">Returns list of Deals.</response>
		[HttpGet]
		[Produces(MediaTypeNames.Application.Json)]
		[ProducesResponseType(typeof(QueryResult<DealListItemDto>), 200)]
		public async Task<IActionResult> GetDeals([FromQuery] GetDealListQuery query)
		{
			return Ok(await Mediator.Send(query));
		}

		/// <summary>
		/// Get Deal by key.
		/// </summary>
		/// <param name="id">DealId.</param>
		/// <response code="200">Returns Deal details.</response>
		/// <response code="404">Deal was not found.</response>
		[HttpGet("{id}")]
		[Produces(MediaTypeNames.Application.Json)]
		[ProducesResponseType(typeof(DealDto), 200)]
		[ProducesResponseType(404)]
		public async Task<IActionResult> GetDeal(int id)
		{
			return Ok(await Mediator.Send(new GetDealQuery(id)));
        }

        /// <summary>
        /// Get a deal including its dependent entities (type, state, contact persons, events, notes, tags).
        /// </summary>
        /// <param name="id">Deal id</param>
        /// <param name="cancellationToken">Cancellation token</param>
        /// <returns>Deal with dependents DTO or 404</returns>
        [HttpGet("{id}/with-dependents")]
        [ProducesResponseType(typeof(DealWithDependentsDto), 200)]
        [ProducesResponseType(404)]
        public async Task<ActionResult<DealWithDependentsDto>> GetWithDependents(int id, CancellationToken cancellationToken)
        {
            var query = new GetDealWithDependentsQuery(id);
            var result = await Mediator.Send(query, cancellationToken);

            return Ok(result);
        }

        /// <summary>
        /// Create Deal.
        /// </summary>
        /// <param name="command">Create Deal Request.</param>
        /// <response code="201">Returns created Deal.</response>
        /// <response code="400">Returns when conditions are not met.</response>
        [HttpPost]
		[Consumes(MediaTypeNames.Application.Json)]
		[Produces(MediaTypeNames.Application.Json)]
		[ProducesResponseType(typeof(DealDto), 201)]
		[ProducesResponseType(typeof(IDictionary<string, string>), 400)]
		public async Task<IActionResult> CreateDeal([FromBody] CreateDealCommand command)
		{
			DealDto result = await Mediator.Send(command);

			return CreatedAtAction("GetDeal", new { id = $"{result.Id}" }, result);
		}

		/// <summary>
		/// Update Deal by key.
		/// </summary>
		/// <param name="id">Can be eg. Z05 (backward compatibility) / gcc-Z05 / goi-328712 / cap-212300.</param>
		/// <param name="command">Update Deal Request.</param>
		/// <response code="200">Returns updated Deal.</response>
		/// <response code="400">Returns when conditions are not met.</response>
		/// <response code="404">Returns when Deal was not found.</response>
		[HttpPut("{id}")]
		[Consumes(MediaTypeNames.Application.Json)]
		[Produces(MediaTypeNames.Application.Json)]
		[ProducesResponseType(typeof(DealDto), 200)]
		[ProducesResponseType(typeof(IDictionary<string, string>), 400)]
		[ProducesResponseType(404)]
		public async Task<IActionResult> UpdateDeal(int id, [FromBody] UpdateDealCommand command)
		{
			command.Id = id;
			DealDto result = await Mediator.Send(command);
			return Ok(result);
		}

		/// <summary>
		/// Delete Deal by key.
		/// </summary>
		/// <param name="id">id from Deal to delete.</param>
		/// <response code="200">Returns deleted Deal.</response>
		/// <response code="400">Returns when conditions are not met.</response>
		/// <response code="404">Returns when Deal was not found.</response>
		[HttpDelete("{id}")]
		[Produces(MediaTypeNames.Application.Json)]
		[ProducesResponseType(204)]
		[ProducesResponseType(404)]
		public async Task<IActionResult> DeleteDeal(int id)
		{
			DealDto result = await Mediator.Send(new DeleteDealCommand(id));
			return Ok(result);
		}

        /// <summary>
        /// Get cached list of deal statuses.
        /// </summary>
        /// <response code="200">Returns cached list of deal statuses.</response>
        [HttpGet("statuses/cached")]
        [Produces(MediaTypeNames.Application.Json)]
        [ProducesResponseType(typeof(IEnumerable<string>), 200)]
        public async Task<IActionResult> GetCachedDealStatuses(CancellationToken cancellationToken)
        {
            var result = await Mediator.Send(new GetCachedDealStatusesQuery(), cancellationToken);
            return Ok(result);
        }
	}
}
