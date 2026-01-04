using MediatR;
using Microsoft.AspNetCore.Mvc;
using System.Net.Mime;
using Sai.DealAssistant.Application.Entities.DealTags.Commands;
using Sai.DealAssistant.Application.Entities.DealTags.Dto;
using Sai.DealAssistant.Application.Entities.DealTags.Queries;
using Sai.DealAssistant.Application.Entities.ContactPersons.Commands;
using AutoMapper;

namespace Sai.DealAssistant.WebApi.Controllers;

public class DealTagsController : BaseController
{
    public DealTagsController(IMediator mediator, IMapper mapper)
        : base(mediator, mapper)
    {
    }

    [HttpGet]
    [Produces(MediaTypeNames.Application.Json)]
    [ProducesResponseType(typeof(IEnumerable<DealTagDto>), 200)]
    public async Task<IActionResult> GetDealTags([FromQuery] GetDealTagsQuery query)
    {
        var result = await Mediator.Send(query);
        return Ok(result);
    }

    [HttpGet("existing")]
    [Produces(MediaTypeNames.Application.Json)]
    [ProducesResponseType(typeof(IEnumerable<string>), 200)]
    public async Task<IActionResult> GetExistingTags()
    {
        var result = await Mediator.Send(new GetCachedTagsQuery());
        return Ok(result);
    }

    [HttpPost]
    [Consumes(MediaTypeNames.Application.Json)]
    [Produces(MediaTypeNames.Application.Json)]
    [ProducesResponseType(typeof(DealTagDto), 201)]
    [ProducesResponseType(typeof(IDictionary<string, string>), 400)]
    public async Task<IActionResult> CreateDealTag([FromBody] AddDealTagIfNotExistsCommand command)
    {
        DealTagDto result = await Mediator.Send(command);
        // Return location to the list endpoint filtered by deal id
        return CreatedAtAction(nameof(GetDealTags), new { dealId = result.DealId }, result);
    }

    [HttpDelete("{id}")]
    [Produces(MediaTypeNames.Application.Json)]
    [ProducesResponseType(typeof(DealTagDto), 200)]
    [ProducesResponseType(404)]
    public async Task<IActionResult> DeleteDealTag(int id)
    {
        DealTagDto result = await Mediator.Send(new DeleteDealTagCommand(id));
        return Ok(result);
    }
}