using AutoMapper;
using MediatR;
using Sai.DealAssistant.Application.Entities.SampleCustomers.Dtos;
using Sai.DealAssistant.Common.Enums;
using Sai.DealAssistant.Domain.Entities;
using Sai.DealAssistant.Domain.Repositories;

namespace Sai.DealAssistant.Application.Entities.SampleCustomers.Queries
{
	public class GetSampleCustomersQuery : PaginatedQueryRequest<QueryResult<SampleCustomerPreviewDto>>
	{
		public GetSampleCustomersQuery()
			: base()
		{
		}

		public GetSampleCustomersQuery(
			string? sortBy,
			SortDirections? sortDirection,
			int? page,
			int? pageSize,
			string? code,
			string? name)
			: base(sortBy, sortDirection, page, pageSize)
		{
			Code = code;
			Name = name;
		}

		public string? Code { get; set; }

		public string? Name { get; set; }

		public class Handler : IRequestHandler<GetSampleCustomersQuery, QueryResult<SampleCustomerPreviewDto>>
		{
			private readonly ISampleCustomerRepository _customerRepository;
			private readonly IMapper _mapper;

			public Handler(ISampleCustomerRepository customerRepository, IMapper mapper)
			{
				_customerRepository = customerRepository;
				_mapper = mapper;
			}

			public async Task<QueryResult<SampleCustomerPreviewDto>> Handle(
				GetSampleCustomersQuery request, CancellationToken cancellationToken)
			{
				int totalItems = await _customerRepository.CountAsync(request.Code, request.Name);
				IReadOnlyCollection<SampleCustomer> customers = await _customerRepository.SelectAsync(
					request.SortBy,
					request.SortDescending,
					request.Page,
					request.PageSize,
					request.Code,
					request.Name);

				return new QueryResult<SampleCustomerPreviewDto>(
					_mapper.Map<IReadOnlyCollection<SampleCustomerPreviewDto>>(customers),
					totalItems);
			}
		}
	}
}
