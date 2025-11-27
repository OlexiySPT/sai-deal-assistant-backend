using AutoMapper;
using FluentValidation;
using MediatR;
using Sai.DealAssistant.Application.Common.Exceptions;
using Sai.DealAssistant.Application.Entities.SampleCustomers.Dtos;
using Sai.DealAssistant.Domain.Entities;
using Sai.DealAssistant.Domain.Repositories;

namespace Sai.DealAssistant.Application.Entities.SampleCustomers.Commands
{
	public class UpdateSampleCustomerCommand : IRequest<SampleCustomerDto>
	{
		public int Id { get; set; }

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

		public class Validator : AbstractValidator<UpdateSampleCustomerCommand>
		{
			private readonly ISampleCustomerRepository _customerRepository;

			public Validator(ISampleCustomerRepository customerRepository)
			{
				_customerRepository = customerRepository;
				RuleFor(c => c.Id)
					.NotEmpty();
				RuleFor(c => c.Code)
					.MustAsync(BeUnique)
					.WithMessage("Code already exist.");
				RuleFor(c => c.Name)
					.NotEmpty();
			}

			private async Task<bool> BeUnique(
				UpdateSampleCustomerCommand command, string code, CancellationToken cancellationToken)
			{
				return !await _customerRepository.ExistsAsync(code);
			}
		}

		public class Handler : IRequestHandler<UpdateSampleCustomerCommand, SampleCustomerDto>
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

			public async Task<SampleCustomerDto> Handle(UpdateSampleCustomerCommand request, CancellationToken cancellationToken)
			{
				SampleCustomer? customer = await _customerRepository.GetAsync(request.Id);
				customer!.Code = request.Code ?? string.Empty;
				customer.Name = request.Name;
				if (customer == null)
				{
					throw new NotFoundException(nameof(SampleCustomer), request.Id);
				}

				SampleCustomer? updatedCustomer = await _customerRepository.UpdateAsync(customer);

				return _mapper.Map<SampleCustomerDto>(updatedCustomer);
			}
		}
	}
}
