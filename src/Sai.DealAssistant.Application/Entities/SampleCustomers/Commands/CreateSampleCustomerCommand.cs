using AutoMapper;
using FluentValidation;
using MediatR;
using Sai.DealAssistant.Application.Common.Exceptions;
using Sai.DealAssistant.Application.Entities.SampleCustomers.Dtos;
using Sai.DealAssistant.Domain.Entities;
using Sai.DealAssistant.Domain.Repositories;
using Sai.DealAssistant.Domain.Repositories.Generic;

namespace Sai.DealAssistant.Application.Entities.SampleCustomers.Commands;

public class CreateSampleCustomerCommand : IRequest<SampleCustomerDto>
{
	public string Code { get; set; } = string.Empty;

	public string Name { get; set; } = string.Empty;

	#region Contact info
	public string? PostalCode { get; set; }

	public string? AddressLn1 { get; set; }

	public string? AddressLn2 { get; set; }

	public string? Country { get; set; }

	public string? Phone { get; set; }

	public string? Email { get; set; }
	#endregion

	#region TaxInfo
	public string? TaxNumber { get; set; }

	public string? VatPayerNumber { get; set; }

	public string? SocialSecurityPayerNumber { get; set; }

	public string? TaxPayerScheme { get; set; }

	public DateTime RegistrationDate { get; set; }
	#endregion

	public class Validator : AbstractValidator<CreateSampleCustomerCommand>
	{
		private readonly IReadRepository<SampleCustomer> _customerRepository;

		public Validator(IReadRepository<SampleCustomer> customerRepository)
		{
			_customerRepository = customerRepository;

			RuleFor(c => c.Code)
				.NotEmpty()
				.MustAsync(async (cmd, code, cToken) => !await _customerRepository.ExistsAsync(p => p.Code == code))
				.WithMessage("Code already exist.");
			RuleFor(c => c.Name)
				.NotEmpty();
		}
	}

	public class Handler : IRequestHandler<CreateSampleCustomerCommand, SampleCustomerDto>
	{
		private readonly ISampleCustomerRepository _customerRepository;
		private readonly IMapper _mapper;

		public Handler(ISampleCustomerRepository customerRepository, IMapper mapper)
		{
			_customerRepository = customerRepository;
			_mapper = mapper;
		}

		public async Task<SampleCustomerDto> Handle(CreateSampleCustomerCommand request, CancellationToken cancellationToken)
		{
			SampleCustomer newCustomer = new SampleCustomer { Code = request.Code, Name = request.Name };

			SampleCustomer? customer = await _customerRepository.CreateAsync(newCustomer);

			if (customer == null)
			{
				throw new NotFoundException(nameof(SampleCustomer), request.Code);
			}

			return _mapper.Map<SampleCustomerDto>(customer);
		}
	}
}
