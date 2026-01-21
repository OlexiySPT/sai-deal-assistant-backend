using AutoMapper;
using MediatR;
using Microsoft.AspNetCore.Mvc;
using Sai.DealAssistant.Application;
using Sai.DealAssistant.Application.Entities.Events.Commands;
using Sai.DealAssistant.Application.Entities.Events.Dtos;
using Sai.DealAssistant.Application.Entities.Events.Queries;
using System.Net.Mime;

namespace Sai.DealAssistant.WebApi.Controllers;

public class EventsController : BaseController
{
    public EventsController(IMediator mediator, IMapper mapper)
        : base(mediator, mapper)
    {
    }

    [HttpGet]
    [Produces(MediaTypeNames.Application.Json)]
    [ProducesResponseType(typeof(QueryResult<EventListItemDto>), 200)]
    public async Task<IActionResult> GetEvents([FromQuery] GetDealEventsQuery query)
    {
        return Ok(await Mediator.Send(query));
    }

    [HttpGet("{id}")]
    [Produces(MediaTypeNames.Application.Json)]
    [ProducesResponseType(typeof(EventDto), 200)]
    [ProducesResponseType(404)]
    public async Task<IActionResult> GetEvent(int id)
    {
        return Ok(await Mediator.Send(new GetEventQuery(id)));
    }

    [HttpPost]
    [Consumes(MediaTypeNames.Application.Json)]
    [Produces(MediaTypeNames.Application.Json)]
    [ProducesResponseType(typeof(EventDto), 201)]
    [ProducesResponseType(typeof(IDictionary<string, string>), 400)]
    public async Task<IActionResult> CreateEvent([FromBody] CreateEventCommand command)
    {
        EventDto result = await Mediator.Send(command);
        return CreatedAtAction("GetEvent", new { id = $"{result.Id}" }, result);
    }

    [HttpPut("{id}")]
    [Consumes(MediaTypeNames.Application.Json)]
    [Produces(MediaTypeNames.Application.Json)]
    [ProducesResponseType(typeof(EventDto), 200)]
    [ProducesResponseType(typeof(IDictionary<string, string>), 400)]
    [ProducesResponseType(404)]
    public async Task<IActionResult> UpdateEvent(int id, [FromBody] UpdateEventCommand command)
    {
        command.Id = id;
        EventDto result = await Mediator.Send(command);
        return Ok(result);
    }

    [HttpDelete("{id}")]
    [Produces(MediaTypeNames.Application.Json)]
    [ProducesResponseType(204)]
    [ProducesResponseType(404)]
    public async Task<IActionResult> DeleteEvent(int id)
    {
        EventDto result = await Mediator.Send(new DeleteEventCommand(id));
        return Ok(result);
    }
}