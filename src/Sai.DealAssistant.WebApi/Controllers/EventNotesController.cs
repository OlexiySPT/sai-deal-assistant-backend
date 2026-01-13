using AutoMapper;
using MediatR;
using Microsoft.AspNetCore.Mvc;
using Sai.DealAssistant.Application;
using Sai.DealAssistant.Application.Entities.EventNotes.Commands;
using Sai.DealAssistant.Application.Entities.EventNotes.Dto;
using Sai.DealAssistant.Application.Entities.EventNotes.Queries;
using System.Net.Mime;

namespace Sai.DealAssistant.WebApi.Controllers;

public class EventNotesController : BaseController
{
    public EventNotesController(IMediator mediator, IMapper mapper)
        : base(mediator, mapper)
    {
    }

    [HttpGet]
    [Produces(MediaTypeNames.Application.Json)]
    [ProducesResponseType(typeof(QueryResult<EventNoteListItemDto>), 200)]
    public async Task<IActionResult> GetEventNotes([FromQuery] GetEventNotesQuery query)
    {
        return Ok(await Mediator.Send(query));
    }

    [HttpGet("{id}")]
    [Produces(MediaTypeNames.Application.Json)]
    [ProducesResponseType(typeof(EventNoteDto), 200)]
    [ProducesResponseType(404)]
    public async Task<IActionResult> GetEventNote(int id)
    {
        return Ok(await Mediator.Send(new GetEventNoteQuery(id)));
    }

    [HttpPost]
    [Consumes(MediaTypeNames.Application.Json)]
    [Produces(MediaTypeNames.Application.Json)]
    [ProducesResponseType(typeof(EventNoteDto), 201)]
    [ProducesResponseType(typeof(IDictionary<string, string>), 400)]
    public async Task<IActionResult> CreateEventNote([FromBody] CreateEventNoteCommand command)
    {
        EventNoteDto result = await Mediator.Send(command);
        return CreatedAtAction("GetEventNote", new { id = $"{result.Id}" }, result);
    }

    [HttpPut("{id}")]
    [Consumes(MediaTypeNames.Application.Json)]
    [Produces(MediaTypeNames.Application.Json)]
    [ProducesResponseType(typeof(EventNoteDto), 200)]
    [ProducesResponseType(typeof(IDictionary<string, string>), 400)]
    [ProducesResponseType(404)]
    public async Task<IActionResult> UpdateEventNote(int id, [FromBody] UpdateEventNoteCommand command)
    {
        command.Id = id;
        EventNoteDto result = await Mediator.Send(command);
        return Ok(result);
    }

    [HttpDelete("{id}")]
    [Produces(MediaTypeNames.Application.Json)]
    [ProducesResponseType(204)]
    [ProducesResponseType(404)]
    public async Task<IActionResult> DeleteEventNote(int id)
    {
        EventNoteDto result = await Mediator.Send(new DeleteEventNoteCommand(id));
        return Ok(result);
    }
}