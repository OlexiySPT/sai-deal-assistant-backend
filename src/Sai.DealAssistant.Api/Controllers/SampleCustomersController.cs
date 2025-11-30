using AutoMapper;
using MediatR;
using Microsoft.AspNetCore.Mvc;
using Sai.DealAssistant.Api.Requests.SampleCustomers;
using Sai.DealAssistant.Application;
using Sai.DealAssistant.Application.Entities.SampleCustomers.Commands;
using Sai.DealAssistant.Application.Entities.SampleCustomers.Dtos;
using Sai.DealAssistant.Application.Entities.SampleCustomers.Queries;
using System.Net.Mime;

namespace Sai.DealAssistant.Api.Controllers
{
	public class SampleCustomersController : BaseController
	{
		public SampleCustomersController(IMediator mediator, IMapper mapper)
			: base(mediator, mapper)
		{
		}

		/// <summary>
		/// Get list of Customers.
		/// </summary>
		/// <remarks>
		/// Uses **scope** of **identity_api.user_read**.
		/// </remarks>
		/// <param name="request">Query parameters.</param>
		/// <response code="200">Returns list of Customers.</response>
		[HttpGet]
		[Produces(MediaTypeNames.Application.Json)]
		[ProducesResponseType(typeof(QueryResult<SampleCustomerPreviewDto>), 200)]
		public async Task<IActionResult> GetCustomers([FromQuery] SearchSampleCustomerRequest request)
		{
			GetSampleCustomersQuery query = new GetSampleCustomersQuery(
				request.SortBy,
				request.SortDirection,
				request.Page,
				request.PageSize,
				request.Code,
				request.Name);

			return Ok(await Mediator.Send(query));
		}

		/// <summary>
		/// Get list of Customers.
		/// </summary>
		/// <remarks>
		/// Uses **scope** of **identity_api.user_read**.
		/// </remarks>
		/// <param name="request">Query parameters.</param>
		/// <response code="200">Returns list of Customers.</response>
		[HttpGet("list-for-accounting")]
		[Produces(MediaTypeNames.Application.Json)]
		[ProducesResponseType(typeof(QueryResult<SampleCustomerPreviewDto>), 200)]
		public async Task<IActionResult> GetCustomersForAccounting([FromQuery] GetSampleCustomersForAccountingQuery request)
		{
			return Ok(await Mediator.Send(request));
		}

		/// <summary>
		/// Get Customer by key.
		/// </summary>
		/// <remarks>
		/// Uses **scope** of **identity_api.user_read**.
		/// </remarks>
		/// <param name="id">Can be eg. Z05 (backward compatibility) / gcc-Z05 / goi-328712 / cap-212300.</param>
		/// <response code="200">Returns Customer details.</response>
		/// <response code="404">Customer was not found.</response>
		[HttpGet("{id}")]
		[Produces(MediaTypeNames.Application.Json)]
		[ProducesResponseType(typeof(SampleCustomerDto), 200)]
		[ProducesResponseType(404)]
		public async Task<IActionResult> GetCustomer(int id)
		{
			return Ok(await Mediator.Send(new GetSampleCustomerQuery(id)));
		}

		/// <summary>
		/// Create Customer.
		/// </summary>
		/// <remarks>
		/// Uses **scope** of **identity_api.user_write**.
		/// </remarks>
		/// <param name="request">Create Customer Request.</param>
		/// <response code="201">Returns created Customer.</response>
		/// <response code="400">Returns when conditions are not met.</response>
		[HttpPost]
		[Consumes(MediaTypeNames.Application.Json)]
		[Produces(MediaTypeNames.Application.Json)]
		[ProducesResponseType(typeof(SampleCustomerDto), 201)]
		[ProducesResponseType(typeof(IDictionary<string, string>), 400)]
		public async Task<IActionResult> CreateCustomer([FromBody] CreateSampleCustomerRequest request)
		{
			CreateSampleCustomerCommand command = Mapper.Map<CreateSampleCustomerCommand>(request);
			SampleCustomerDto customer = await Mediator.Send(command);

			return CreatedAtAction("GetCustomer", new { id = $"{customer.Id}" }, customer);
		}

		/// <summary>
		/// Update Customer by key.
		/// </summary>
		/// <remarks>
		/// Uses **scope** of **identity_api.user_write**.
		/// </remarks>
		/// <param name="id">Can be eg. Z05 (backward compatibility) / gcc-Z05 / goi-328712 / cap-212300.</param>
		/// <param name="request">Update Customer Request.</param>
		/// <response code="200">Returns updated Customer.</response>
		/// <response code="400">Returns when conditions are not met.</response>
		/// <response code="404">Returns when Customer was not found.</response>
		[HttpPut("{id}")]
		[Consumes(MediaTypeNames.Application.Json)]
		[Produces(MediaTypeNames.Application.Json)]
		[ProducesResponseType(typeof(SampleCustomerDto), 200)]
		[ProducesResponseType(typeof(IDictionary<string, string>), 400)]
		[ProducesResponseType(404)]
		public async Task<IActionResult> UpdateCustomer(int id, [FromBody] UpdateSampleCustomerRequest request)
		{
			UpdateSampleCustomerCommand command = Mapper.Map<UpdateSampleCustomerCommand>(request);

			command.Id = id;

			SampleCustomerDto customer = await Mediator.Send(command);

			return Ok(customer);
		}
	}
}
