using MediatR;
using Sai.DealAssistant.Application.Entities.SampleCustomers.Dtos;
using Sai.DealAssistant.Domain.Entities;
using Sai.DealAssistant.Domain.Repositories.Generic;

namespace Sai.DealAssistant.Application.Entities.SampleCustomers.Queries
{
	public class GetSampleCustomersForAccountingQuery : IRequest<QueryResult<SampleCustomerForAccountingDto>>
	{
		public string? Country { get; set; }

		public class Handler : IRequestHandler<GetSampleCustomersForAccountingQuery, QueryResult<SampleCustomerForAccountingDto>>
		{
			private readonly IReadRepository<SampleCustomer> _customerRepository;

			public Handler(IReadRepository<SampleCustomer> customerRepository)
			{
				_customerRepository = customerRepository;
			}

			public async Task<QueryResult<SampleCustomerForAccountingDto>> Handle(
				GetSampleCustomersForAccountingQuery request, CancellationToken cancellationToken)
			{
				IQueryable<SampleCustomer> qry = _customerRepository.GetAll();
				if (request.Country != null && !string.IsNullOrEmpty(request.Country))
				{
					qry = qry.Where(p => p.Country == request.Country);
				}

				int totalItems = await _customerRepository.CountAsync(qry);
				IReadOnlyCollection<SampleCustomerForAccountingDto> result = await _customerRepository.SelectAsync(
					qry,
					p => new SampleCustomerForAccountingDto
					{
						Id = p.Id,
						Code = p.Code,
						Name = p.Name
					});
				return new QueryResult<SampleCustomerForAccountingDto>(result, totalItems);
			}
		}
	}
}
