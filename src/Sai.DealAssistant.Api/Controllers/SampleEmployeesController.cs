using AutoMapper;
using MediatR;
using Microsoft.AspNetCore.Mvc;
using Sai.DealAssistant.Application;
using Sai.DealAssistant.Application.Entities.SampleEmployees.Commands;
using Sai.DealAssistant.Application.Entities.SampleEmployees.Dtos;
using Sai.DealAssistant.Application.Entities.SampleEmployees.Queries;
using System.Net.Mime;

namespace Sai.DealAssistant.Api.Controllers
{
	public class SampleEmployeesController : BaseController
	{
		public SampleEmployeesController(IMediator mediator, IMapper mapper)
			: base(mediator, mapper)
		{
		}

		/// <summary>
		/// Get list of Employees.
		/// </summary>
		/// <param name="query">Query parameters.</param>
		/// <response code="200">Returns list of Employees.</response>
		[HttpGet]
		[Produces(MediaTypeNames.Application.Json)]
		[ProducesResponseType(typeof(QueryResult<SampleEmployeePreviewDto>), 200)]
		public async Task<IActionResult> GetEmployees([FromQuery] GetSampleEmployeesQuery query)
		{
			return Ok(await Mediator.Send(query));
		}

		/// <summary>
		/// Get Employee by key.
		/// </summary>
		/// <param name="id">EmployeeId.</param>
		/// <response code="200">Returns Employee details.</response>
		/// <response code="404">Employee was not found.</response>
		[HttpGet("{id}")]
		[Produces(MediaTypeNames.Application.Json)]
		[ProducesResponseType(typeof(SampleEmployeeDto), 200)]
		[ProducesResponseType(404)]
		public async Task<IActionResult> GetEmployee(int id)
		{
			return Ok(await Mediator.Send(new GetSampleEmployeeQuery(id)));
		}

		/// <summary>
		/// Create Employee.
		/// </summary>
		/// <param name="command">Create Employee Request.</param>
		/// <response code="201">Returns created Employee.</response>
		/// <response code="400">Returns when conditions are not met.</response>
		[HttpPost]
		[Consumes(MediaTypeNames.Application.Json)]
		[Produces(MediaTypeNames.Application.Json)]
		[ProducesResponseType(typeof(SampleEmployeeDto), 201)]
		[ProducesResponseType(typeof(IDictionary<string, string>), 400)]
		public async Task<IActionResult> CreateEmployee([FromBody] CreateSampleEmployeeCommand command)
		{
			SampleEmployeeDto employee = await Mediator.Send(command);

			return CreatedAtAction("GetEmployee", new { id = $"{employee.Id}" }, employee);
		}

		/// <summary>
		/// Update Employee by key.
		/// </summary>
		/// <param name="id">Can be eg. Z05 (backward compatibility) / gcc-Z05 / goi-328712 / cap-212300.</param>
		/// <param name="command">Update Employee Request.</param>
		/// <response code="200">Returns updated Employee.</response>
		/// <response code="400">Returns when conditions are not met.</response>
		/// <response code="404">Returns when Employee was not found.</response>
		[HttpPut("{id}")]
		[Consumes(MediaTypeNames.Application.Json)]
		[Produces(MediaTypeNames.Application.Json)]
		[ProducesResponseType(typeof(SampleEmployeeDto), 200)]
		[ProducesResponseType(typeof(IDictionary<string, string>), 400)]
		[ProducesResponseType(404)]
		public async Task<IActionResult> UpdateEmployee(int id, [FromBody] UpdateSampleEmployeeCommand command)
		{
			command.Id = id;
			SampleEmployeeDto employee = await Mediator.Send(command);
			return Ok(employee);
		}

		/// <summary>
		/// Delete Employee by key.
		/// </summary>
		/// <param name="id">id from Employee to delete.</param>
		/// <response code="200">Returns deleted Employee.</response>
		/// <response code="400">Returns when conditions are not met.</response>
		/// <response code="404">Returns when Employee was not found.</response>
		[HttpDelete("{id}")]
		[Produces(MediaTypeNames.Application.Json)]
		[ProducesResponseType(204)]
		[ProducesResponseType(404)]
		public async Task<IActionResult> DeleteEmployee(int id)
		{
			await Mediator.Send(new DeleteSampleEmployeeCommand(id));
			return NoContent();
		}
	}
}
