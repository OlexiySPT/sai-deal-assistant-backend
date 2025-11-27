using AutoMapper;
using MediatR;
using Sai.DealAssistant.Application.Common.Exceptions;
using Sai.DealAssistant.Application.Entities.SampleCustomers.Dtos;
using Sai.DealAssistant.Domain.Entities;
using Sai.DealAssistant.Domain.Repositories;

namespace Sai.DealAssistant.Application.Entities.SampleCustomers.Queries;

public class GetSampleCustomerQuery : IRequest<SampleCustomerDto>
{
	public GetSampleCustomerQuery(int id)
	{
		Id = id;
	}

	public int Id { get; }

	public class Handler : IRequestHandler<GetSampleCustomerQuery, SampleCustomerDto>
	{
		private readonly ISampleCustomerRepository _customerRepository;
		private readonly IMapper _mapper;

		public Handler(
			ISampleCustomerRepository customerRepository,
			IMapper mapper)
		{
			_customerRepository = customerRepository;
			_mapper = mapper;
		}

		public async Task<SampleCustomerDto> Handle(GetSampleCustomerQuery request, CancellationToken cancellationToken)
		{
			SampleCustomer? customer = await _customerRepository.GetAsync(request.Id);

			if (customer == null)
			{
				throw new NotFoundException(nameof(SampleCustomer), request.Id);
			}

			return _mapper.Map<SampleCustomerDto>(customer);
		}
	}
}
