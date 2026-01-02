using AutoMapper;
using MediatR;
using Microsoft.AspNetCore.Mvc;
using Sai.DealAssistant.Application;
using Sai.DealAssistant.Application.Entities.DealContactReps.Commands;
using Sai.DealAssistant.Application.Entities.DealContactReps.Dtos;
using Sai.DealAssistant.Application.Entities.DealContactReps.Queries;
using System.Net.Mime;

namespace Sai.DealAssistant.WebApi.Controllers;

public class DealContactRepsController : BaseController
{
    public DealContactRepsController(IMediator mediator, IMapper mapper)
        : base(mediator, mapper)
    {
    }

    [HttpGet]
    [Produces(MediaTypeNames.Application.Json)]
    [ProducesResponseType(typeof(QueryResult<DealContactRepListItemDto>), 200)]
    public async Task<IActionResult> GetDealContactReps([FromQuery] GetDealContactRepsQuery query)
    {
        return Ok(await Mediator.Send(query));
    }

    [HttpGet("{id}")]
    [Produces(MediaTypeNames.Application.Json)]
    [ProducesResponseType(typeof(DealContactRepDto), 200)]
    [ProducesResponseType(404)]
    public async Task<IActionResult> GetDealContactRep(int id)
    {
        return Ok(await Mediator.Send(new GetDealContactRepQuery(id)));
    }

    [HttpPost]
    [Consumes(MediaTypeNames.Application.Json)]
    [Produces(MediaTypeNames.Application.Json)]
    [ProducesResponseType(typeof(DealContactRepDto), 201)]
    [ProducesResponseType(typeof(IDictionary<string, string>), 400)]
    public async Task<IActionResult> CreateDealContactRep([FromBody] CreateDealContactRepCommand command)
    {
        DealContactRepDto result = await Mediator.Send(command);
        return CreatedAtAction("GetDealContactRep", new { id = $"{result.Id}" }, result);
    }

    [HttpPut("{id}")]
    [Consumes(MediaTypeNames.Application.Json)]
    [Produces(MediaTypeNames.Application.Json)]
    [ProducesResponseType(typeof(DealContactRepDto), 200)]
    [ProducesResponseType(typeof(IDictionary<string, string>), 400)]
    [ProducesResponseType(404)]
    public async Task<IActionResult> UpdateDealContactRep(int id, [FromBody] UpdateDealContactRepCommand command)
    {
        command.Id = id;
        DealContactRepDto result = await Mediator.Send(command);
        return Ok(result);
    }

    [HttpDelete("{id}")]
    [Produces(MediaTypeNames.Application.Json)]
    [ProducesResponseType(204)]
    [ProducesResponseType(404)]
    public async Task<IActionResult> DeleteDealContactRep(int id)
    {
        DealContactRepDto result = await Mediator.Send(new DeleteDealContactRepCommand(id));
        return Ok(result);
    }
}