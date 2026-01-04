using AutoMapper;
using MediatR;
using Microsoft.AspNetCore.Mvc;
using Sai.DealAssistant.Application;
using Sai.DealAssistant.Application.Entities.ContactPersons.Commands;
using Sai.DealAssistant.Application.Entities.ContactPersons.Dto;
using Sai.DealAssistant.Application.Entities.ContactPersons.Queries;
using Sai.DealAssistant.Application.Entities.EventNotes.Commands;
using System.Net.Mime;

namespace Sai.DealAssistant.WebApi.Controllers;

public class ContactPersonsController : BaseController
{
    public ContactPersonsController(IMediator mediator, IMapper mapper)
        : base(mediator, mapper)
    {
    }

    [HttpGet]
    [Produces(MediaTypeNames.Application.Json)]
    [ProducesResponseType(typeof(QueryResult<ContactPersonListItemDto>), 200)]
    public async Task<IActionResult> GetDealContactReps([FromQuery] GetDealContactPersonsQuery query)
    {
        return Ok(await Mediator.Send(query));
    }

    [HttpGet("{id}")]
    [Produces(MediaTypeNames.Application.Json)]
    [ProducesResponseType(typeof(ContactPersonDto), 200)]
    [ProducesResponseType(404)]
    public async Task<IActionResult> GetDealContactRep(int id)
    {
        return Ok(await Mediator.Send(new GetContactPersonQuery(id)));
    }

    [HttpPost]
    [Consumes(MediaTypeNames.Application.Json)]
    [Produces(MediaTypeNames.Application.Json)]
    [ProducesResponseType(typeof(ContactPersonDto), 201)]
    [ProducesResponseType(typeof(IDictionary<string, string>), 400)]
    public async Task<IActionResult> CreateDealContactRep([FromBody] CreateContactPersonCommand command)
    {
        ContactPersonDto result = await Mediator.Send(command);
        return CreatedAtAction("GetDealContactRep", new { id = $"{result.Id}" }, result);
    }

    [HttpPut("{id}")]
    [Consumes(MediaTypeNames.Application.Json)]
    [Produces(MediaTypeNames.Application.Json)]
    [ProducesResponseType(typeof(ContactPersonDto), 200)]
    [ProducesResponseType(typeof(IDictionary<string, string>), 400)]
    [ProducesResponseType(404)]
    public async Task<IActionResult> UpdateDealContactRep(int id, [FromBody] UpdateContactPersonCommand command)
    {
        command.Id = id;
        ContactPersonDto result = await Mediator.Send(command);
        return Ok(result);
    }

    [HttpDelete("{id}")]
    [Produces(MediaTypeNames.Application.Json)]
    [ProducesResponseType(204)]
    [ProducesResponseType(404)]
    public async Task<IActionResult> DeleteDealContactRep(int id)
    {
        ContactPersonDto result = await Mediator.Send(new DeleteContactPersonCommand(id));
        return Ok(result);
    }
}