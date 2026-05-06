using AutoMapper;
using MediatR;
using Microsoft.AspNetCore.Mvc;
using Sai.DealAssistant.Application.Entities.AiPrompts.Commands;
using Sai.DealAssistant.Application.Entities.AiPrompts.Dtos;
using Sai.DealAssistant.Application.Entities.AiPrompts.Queries;
using Sai.DealAssistant.Application;
using System.Net.Mime;

namespace Sai.DealAssistant.WebApi.Controllers;

public class AiPromptsController : BaseController
{
    public AiPromptsController(IMediator mediator, IMapper mapper)
        : base(mediator, mapper)
    {
    }

    [HttpGet]
    [Produces(MediaTypeNames.Application.Json)]
    [ProducesResponseType(typeof(QueryResult<AiPromptDto>), 200)]
    public async Task<IActionResult> GetAiPrompts([FromQuery] GetAiPromptListQuery query)
    {
        return Ok(await Mediator.Send(query));
    }

    [HttpGet("{id}")]
    [Produces(MediaTypeNames.Application.Json)]
    [ProducesResponseType(typeof(AiPromptDto), 200)]
    [ProducesResponseType(404)]
    public async Task<IActionResult> GetAiPrompt(int id)
    {
        return Ok(await Mediator.Send(new GetAiPromptQuery(id)));
    }

    [HttpPost]
    [Consumes(MediaTypeNames.Application.Json)]
    [Produces(MediaTypeNames.Application.Json)]
    [ProducesResponseType(typeof(AiPromptDto), 201)]
    [ProducesResponseType(typeof(IDictionary<string, string>), 400)]
    public async Task<IActionResult> CreateAiPrompt([FromBody] CreateAiPromptCommand command)
    {
        AiPromptDto result = await Mediator.Send(command);
        return CreatedAtAction("GetAiPrompt", new { id = result.Id }, result);
    }

    [HttpPut("{id}")]
    [Consumes(MediaTypeNames.Application.Json)]
    [Produces(MediaTypeNames.Application.Json)]
    [ProducesResponseType(typeof(AiPromptDto), 200)]
    [ProducesResponseType(typeof(IDictionary<string, string>), 400)]
    [ProducesResponseType(404)]
    public async Task<IActionResult> UpdateAiPrompt(int id, [FromBody] UpdateAiPromptCommand command)
    {
        command.Id = id;
        AiPromptDto result = await Mediator.Send(command);
        return Ok(result);
    }
}
