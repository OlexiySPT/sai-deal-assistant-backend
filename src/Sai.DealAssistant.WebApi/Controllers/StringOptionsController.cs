using MediatR;
using Microsoft.AspNetCore.Mvc;
using Sai.DealAssistant.Application.Entities.General.Queries;
using Sai.DealAssistant.Domain.Repositories.Generic;
using System.Net.Mime;

namespace Sai.DealAssistant.WebApi.Controllers;

[ApiController]
[Route("api/[controller]")]
public class StringOptionsController : ControllerBase
{
    private readonly IMediator _mediator;

    public StringOptionsController(IMediator mediator)
    {
        _mediator = mediator;
    }

    /// <summary>
    /// Get cached string options for any entity and field.
    /// </summary>
    /// <param name="entityType">Entity type name (e.g. 'Deal').</param>
    /// <param name="fieldName">Field name (e.g. 'Status').</param>
    /// <param name="sortDirection">Sort direction (optional, default Ascending).</param>
    /// <response code="200">Returns cached string options.</response>
    [HttpGet]
    [Produces(MediaTypeNames.Application.Json)]
    [ProducesResponseType(typeof(IEnumerable<string>), 200)]
    public async Task<IActionResult> GetOptions(
        [FromQuery] string entityType,
        [FromQuery] string fieldName,
        [FromQuery] string? sortDirection = null)
    {
        if (string.IsNullOrWhiteSpace(entityType) || string.IsNullOrWhiteSpace(fieldName))
            return BadRequest("entityType and fieldName are required.");

        // Resolve the Type from the entityType string
        var type = AppDomain.CurrentDomain.GetAssemblies()
            .SelectMany(a => a.GetTypes())
            .FirstOrDefault(t => t.Name.Equals(entityType, StringComparison.OrdinalIgnoreCase));
        if (type == null)
            return BadRequest($"Entity type '{entityType}' not found.");

        SortDirection parsedSort = SortDirection.Ascending;
        if (!string.IsNullOrWhiteSpace(sortDirection))
        {
            if (!Enum.TryParse<SortDirection>(sortDirection, true, out parsedSort))
                return BadRequest($"Invalid sortDirection value: {sortDirection}");
        }

        var query = new GetCachedStringOptions
        {
            EntityType = type,
            FieldName = fieldName,
            SortDirection = parsedSort
        };
        var result = await _mediator.Send(query);
        return Ok(result);
    }
}
