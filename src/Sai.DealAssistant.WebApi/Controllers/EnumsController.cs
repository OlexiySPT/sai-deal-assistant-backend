using AutoMapper;
using MediatR;
using Microsoft.AspNetCore.Mvc;
using Sai.DealAssistant.Application.Entities.Enums.Queries;
using System.Net.Mime;

namespace Sai.DealAssistant.WebApi.Controllers;
public class EnumsController : BaseController
{
    public EnumsController(IMediator mediator, IMapper mapper)
        : base(mediator, mapper)
    {
    }

    // GET api/enums
    [HttpGet]
    [Produces(MediaTypeNames.Application.Json)]
    [ProducesResponseType(typeof(IEnumerable<string>), 200)]
    public async Task<IActionResult> GetAvailable()
    {
        var result = await Mediator.Send(new GetAvailableEnumsQuery());
        return Ok(result);
    }

    // GET api/enums/{enumName}
    [HttpGet("{enumName}")]
    [Produces(MediaTypeNames.Application.Json)]
    [ProducesResponseType(typeof(IEnumerable<IDictionary<string, object?>>), 200)]
    [ProducesResponseType(400)]
    [ProducesResponseType(404)]
    public async Task<IActionResult> Get(string enumName, CancellationToken cancellationToken)
    {
        var result = await Mediator.Send(new GetEnumItemsQuery { EnumName = enumName }, cancellationToken);
        return Ok(result);
    }
}