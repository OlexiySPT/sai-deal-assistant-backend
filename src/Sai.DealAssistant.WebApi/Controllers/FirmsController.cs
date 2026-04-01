using AutoMapper;
using MediatR;
using Microsoft.AspNetCore.Mvc;
using Sai.DealAssistant.Application;
using Sai.DealAssistant.Application.Entities.Firms.Commands;
using Sai.DealAssistant.Application.Entities.Firms.Dtos;
using Sai.DealAssistant.Application.Entities.Firms.Queries;
using System.Net.Mime;

namespace Sai.DealAssistant.WebApi.Controllers;

public class FirmsController : BaseController
{
	public FirmsController(IMediator mediator, IMapper mapper)
		: base(mediator, mapper)
	{
	}

	/// <summary>
	/// Get list of Firms.
	/// </summary>
	/// <param name="query">Query parameters.</param>
	/// <response code="200">Returns list of Firms.</response>
	[HttpGet]
	[Produces(MediaTypeNames.Application.Json)]
	[ProducesResponseType(typeof(QueryResult<FirmListItemDto>), 200)]
	public async Task<IActionResult> GetFirms([FromQuery] GetFirmListQuery query)
	{
		return Ok(await Mediator.Send(query));
	}

	/// <summary>
	/// Get Firm by key.
	/// </summary>
	/// <param name="id">FirmId.</param>
	/// <response code="200">Returns Firm details.</response>
	/// <response code="404">Firm was not found.</response>
	[HttpGet("{id}")]
	[Produces(MediaTypeNames.Application.Json)]
	[ProducesResponseType(typeof(FirmDto), 200)]
	[ProducesResponseType(404)]
	public async Task<IActionResult> GetFirm(int id)
	{
		return Ok(await Mediator.Send(new GetFirmQuery(id)));
	}

	/// <summary>
	/// Get Firm including its contact persons.
	/// </summary>
	/// <param name="id">FirmId.</param>
	/// <param name="cancellationToken">Cancellation token.</param>
	/// <response code="200">Returns Firm with its contact persons.</response>
	/// <response code="404">Firm was not found.</response>
	[HttpGet("{id}/with-dependents")]
	[Produces(MediaTypeNames.Application.Json)]
	[ProducesResponseType(typeof(FirmWithDependenciesDto), 200)]
	[ProducesResponseType(404)]
	public async Task<ActionResult<FirmWithDependenciesDto>> GetFirmWithDependents(int id, CancellationToken cancellationToken)
	{
		var result = await Mediator.Send(new GetFirmWithDependentsQuery(id), cancellationToken);
		return Ok(result);
	}

	/// <summary>
	/// Create Firm.
	/// </summary>
	/// <param name="command">Create Firm Request.</param>
	/// <response code="201">Returns created Firm.</response>
	/// <response code="400">Returns when conditions are not met.</response>
	[HttpPost]
	[Consumes(MediaTypeNames.Application.Json)]
	[Produces(MediaTypeNames.Application.Json)]
	[ProducesResponseType(typeof(FirmDto), 201)]
	[ProducesResponseType(typeof(IDictionary<string, string>), 400)]
	public async Task<IActionResult> CreateFirm([FromBody] CreateFirmCommand command)
	{
		FirmDto result = await Mediator.Send(command);
		return CreatedAtAction("GetFirm", new { id = $"{result.Id}" }, result);
	}

	/// <summary>
	/// Update Firm by key.
	/// </summary>
	/// <param name="id">FirmId.</param>
	/// <param name="command">Update Firm Request.</param>
	/// <response code="200">Returns updated Firm.</response>
	/// <response code="400">Returns when conditions are not met.</response>
	/// <response code="404">Returns when Firm was not found.</response>
	[HttpPut("{id}")]
	[Consumes(MediaTypeNames.Application.Json)]
	[Produces(MediaTypeNames.Application.Json)]
	[ProducesResponseType(typeof(FirmDto), 200)]
	[ProducesResponseType(typeof(IDictionary<string, string>), 400)]
	[ProducesResponseType(404)]
	public async Task<IActionResult> UpdateFirm(int id, [FromBody] UpdateFirmCommand command)
	{
		command.Id = id;
		FirmDto result = await Mediator.Send(command);
		return Ok(result);
	}

	/// <summary>
	/// Delete Firm by key.
	/// </summary>
	/// <param name="id">FirmId to delete.</param>
	/// <response code="200">Returns deleted Firm.</response>
	/// <response code="400">Returns when conditions are not met.</response>
	/// <response code="404">Returns when Firm was not found.</response>
	[HttpDelete("{id}")]
	[Produces(MediaTypeNames.Application.Json)]
	[ProducesResponseType(204)]
	[ProducesResponseType(404)]
	public async Task<IActionResult> DeleteFirm(int id)
	{
		FirmDto result = await Mediator.Send(new DeleteFirmCommand(id));
		return Ok(result);
	}
}