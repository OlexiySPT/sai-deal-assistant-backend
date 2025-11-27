using AutoMapper;
using MediatR;
using Sai.DealAssistant.Application.Common.Exceptions;
using Sai.DealAssistant.Application.Entities.SampleEmployees.Dtos;
using Sai.DealAssistant.Domain.Entities;
using Sai.DealAssistant.Domain.Repositories.Generic;

namespace Sai.DealAssistant.Application.Entities.SampleEmployees.Commands
{
	public class DeleteSampleEmployeeCommand : IRequest<SampleEmployeeDto>
	{
		public DeleteSampleEmployeeCommand(int id)
		{
			Id = id;
		}

		public int Id { get; set; }

		public class Handler : IRequestHandler<DeleteSampleEmployeeCommand, SampleEmployeeDto>
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

			public async Task<SampleEmployeeDto> Handle(DeleteSampleEmployeeCommand request, CancellationToken cancellationToken)
			{
				SampleEmployee? deletedEmployee = await _repository.DeleteAsync(request.Id);

				if (deletedEmployee == null)
				{
					throw new NotFoundException(nameof(SampleEmployee), request.Id);
				}

				return _mapper.Map<SampleEmployeeDto>(deletedEmployee);
			}
		}
	}
}
