using MediatR;
using Microsoft.AspNetCore.Mvc;
using Sai.DealAssistant.Application.Entities.General.Commands;
using System.Net.Mime;

namespace Sai.DealAssistant.WebApi.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    [Produces(MediaTypeNames.Application.Json)]
    public class FieldUpdateController : ControllerBase
    {
        private readonly IMediator _mediator;

        public FieldUpdateController(IMediator mediator)
        {
            _mediator = mediator;
        }

        /// <summary>
        /// Updates a string field of an entity.
        /// </summary>
        /// <param name="command"></param>
        /// <returns>The updated value.</returns>
        [HttpPut("string")]
        [Consumes(MediaTypeNames.Application.Json)]
        [ProducesResponseType(typeof(string), StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public async Task<IActionResult> UpdateStringField([FromBody] UpdateStringFieldCommand command)
        {
            var result = await _mediator.Send(command);
            return Ok(result);
        }

        /// <summary>
        /// Updates a numeric field of an entity.
        /// </summary>
        /// <param name="command"></param>
        /// <returns>The updated value.</returns>
        [HttpPut("numeric")]
        [Consumes(MediaTypeNames.Application.Json)]
        [ProducesResponseType(typeof(decimal?), StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public async Task<IActionResult> UpdateNumericField([FromBody] UpdateNumericFieldCommand command)
        {
            var result = await _mediator.Send(command);
            return Ok(result);
        }

        /// <summary>
        /// Updates a date field of an entity.
        /// </summary>
        /// <param name="command"></param>
        /// <returns>The updated value.</returns>
        [HttpPut("date")]
        [Consumes(MediaTypeNames.Application.Json)]
        [ProducesResponseType(typeof(DateTimeOffset?), StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public async Task<IActionResult> UpdateDateField([FromBody] UpdateDateFieldCommand command)
        {
            var result = await _mediator.Send(command);
            return Ok(result);
        }
    }
}