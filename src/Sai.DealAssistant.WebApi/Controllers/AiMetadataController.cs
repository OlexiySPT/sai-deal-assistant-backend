using AutoMapper;
using MediatR;
using Microsoft.AspNetCore.Mvc;
using Sai.DealAssistant.Application;
using Sai.DealAssistant.Application.Entities.AiMetadatas.Commands;
using Sai.DealAssistant.Application.Entities.AiMetadatas.Dtos;
using Sai.DealAssistant.Application.Entities.AiMetadatas.Queries;
using System.Net.Mime;

namespace Sai.DealAssistant.WebApi.Controllers;

public class AiMetadataController : BaseController
{
    public AiMetadataController(IMediator mediator, IMapper mapper)
        : base(mediator, mapper)
    {
    }

    [HttpGet]
    [Produces(MediaTypeNames.Application.Json)]
    [ProducesResponseType(typeof(QueryResult<AiMetadataDto>), 200)]
    public async Task<IActionResult> GetAiMetadata([FromQuery] GetAiMetadataListQuery query)
    {
        return Ok(await Mediator.Send(query));
    }

    [HttpGet("{id}")]
    [Produces(MediaTypeNames.Application.Json)]
    [ProducesResponseType(typeof(AiMetadataDto), 200)]
    [ProducesResponseType(404)]
    public async Task<IActionResult> GetAiMetadata(int id)
    {
        return Ok(await Mediator.Send(new GetAiMetadataQuery(id)));
    }

    [HttpPost]
    [Consumes(MediaTypeNames.Application.Json)]
    [Produces(MediaTypeNames.Application.Json)]
    [ProducesResponseType(typeof(AiMetadataDto), 201)]
    [ProducesResponseType(typeof(IDictionary<string, string>), 400)]
    public async Task<IActionResult> CreateAiMetadata([FromBody] CreateAiMetadataCommand command)
    {
        AiMetadataDto result = await Mediator.Send(command);
        return CreatedAtAction("GetAiMetadata", new { id = result.Id }, result);
    }

    [HttpPut("{id}")]
    [Consumes(MediaTypeNames.Application.Json)]
    [Produces(MediaTypeNames.Application.Json)]
    [ProducesResponseType(typeof(AiMetadataDto), 200)]
    [ProducesResponseType(typeof(IDictionary<string, string>), 400)]
    [ProducesResponseType(404)]
    public async Task<IActionResult> UpdateAiMetadata(int id, [FromBody] UpdateAiMetadataCommand command)
    {
        command.Id = id;
        AiMetadataDto result = await Mediator.Send(command);
        return Ok(result);
    }
}
