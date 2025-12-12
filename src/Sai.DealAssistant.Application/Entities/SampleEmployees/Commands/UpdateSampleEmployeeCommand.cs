using AutoMapper;
using FluentValidation;
using MediatR;
using Sai.DealAssistant.Application.Common.Exceptions;
using Sai.DealAssistant.Application.Entities.SampleEmployees.Dtos;
using Sai.DealAssistant.Domain.Entities.Samples;
using Sai.DealAssistant.Domain.Repositories.Generic;

namespace Sai.DealAssistant.Application.Entities.SampleEmployees.Commands
{
	public class UpdateSampleEmployeeCommand : IRequest<SampleEmployeeDto>
	{
		public UpdateSampleEmployeeCommand(
			int id,
			int customerId,
			string firstName,
			string lastName,
			string? email = null)
		{
			Id = id;
			CustomerId = customerId;
			FirstName = firstName;
			LastName = lastName;
			Email = email;
		}

		public int Id { get; set; }

		public int CustomerId { get; }

		public string FirstName { get; }

		public string LastName { get; }

		public string? Email { get; }

		public class MappingProfile : Profile
		{
			public MappingProfile()
			{
				CreateMap<UpdateSampleEmployeeCommand, SampleEmployee>();
			}
		}

		public class Validator : AbstractValidator<CreateSampleEmployeeCommand>
		{
			private readonly IReadRepository<SampleCustomer> _customerRepository;
			private readonly IReadRepository<SampleEmployee> _employeeRepository;

			public Validator(IReadRepository<SampleCustomer> customerRepository, IReadRepository<SampleEmployee> employeeRepository)
			{
				_customerRepository = customerRepository;
				_employeeRepository = employeeRepository;

				RuleFor(c => c.CustomerId)
					.GreaterThan(0)
					.MustAsync(async (cmd, customerId, cToken) => await _customerRepository.ExistsAsync(p => p.Id == customerId))
					.WithMessage("Customer does not exist.");
				RuleFor(p => p.Email)
					.NotEmpty()
					.MustAsync(async (cmd, email, cToken) => !await _employeeRepository.ExistsAsync(p => p.Email == email))
					.WithMessage("Employee with this email already exists");
				RuleFor(c => c.FirstName)
					.NotEmpty();
				RuleFor(c => c.LastName)
					.NotEmpty();
			}
		}

		public class Handler : IRequestHandler<UpdateSampleEmployeeCommand, SampleEmployeeDto>
		{
			private readonly ICrudRepository<SampleEmployee> _repository;
			private readonly IMapper _mapper;

			public Handler(
				ICrudRepository<SampleEmployee> customerRepository,
				IMapper mapper)
			{
				_repository = customerRepository;
				_mapper = mapper;
			}

			public async Task<SampleEmployeeDto> Handle(UpdateSampleEmployeeCommand request, CancellationToken cancellationToken)
			{
				SampleEmployee employeeToUpdate = _mapper.Map<SampleEmployee>(request);

				SampleEmployee? updatedEmployee = await _repository.UpdateAsync(employeeToUpdate);

				if (updatedEmployee == null)
				{
					throw new NotFoundException(nameof(SampleEmployee), request.Id);
				}

				return _mapper.Map<SampleEmployeeDto>(updatedEmployee);
			}
		}
	}
}
