using MediatR;
using Microsoft.AspNetCore.Http.HttpResults;
using Microsoft.AspNetCore.Mvc;
using Sai.DealAssistant.Application.DealAutomation.Commands;
using Sai.DealAssistant.Common.JobQueue;
using Sai.DealAssistant.Common.Queue;
using System.Net.Mime;

namespace Sai.DealAssistant.WebApi.Controllers;

[ApiController]
[Route("api/[controller]")]
public class DealAutomationController : ControllerBase
{
    private readonly IJobQueue<IJobQueueCommand> _queue;
    private readonly IMediator _mediator;

    public DealAutomationController(IMediator mediator, IJobQueue<IJobQueueCommand> queue)
    {
        _mediator = mediator;
        _queue = queue;
    }

    /// <summary>
    /// Reads and returns the HTML content of the specified page using automation.
    /// </summary>
    /// <param name="url">The URL of the page to read.</param>
    /// <response code="200">Returns the HTML content of the page.</response>
    /// <response code="400">URL must be provided.</response>
    [HttpPost("ReadPage")]
    [Produces("application/json")]
    [ProducesResponseType(typeof(string), 200)]
    [ProducesResponseType(400)]
    public async Task<IActionResult> ReadPage([FromBody] ProcessPageCommand command)
    {
        var result = await _mediator.Send(command);
        return Ok(result);
    }

    /// <summary>
    /// Generate cover letter for the deal by id.
    /// </summary>
    /// <param name="id">Deal id</param>
    /// <response code="200">Returns generated cover letter string.</response>
    /// <response code="400">Returns when conditions are not met.</response>
    /// <response code="404">Deal was not found.</response>
    [HttpPost("{id}/generate-cover-letter")]
    [Produces(MediaTypeNames.Application.Json)]
    [ProducesResponseType(typeof(string), 200)]
    [ProducesResponseType(typeof(IDictionary<string, string>), 400)]
    [ProducesResponseType(404)]
    public async Task<IActionResult> GenerateCoverLetter(int id)
    {
        var command = new GenerateCoverLetterCommand { DealId = id };
        var result = await _mediator.Send(command);
        return Ok(result);
    }
}
