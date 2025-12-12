using AutoMapper;
using MediatR;
using Sai.DealAssistant.Application.Common.Exceptions;
using Sai.DealAssistant.Application.Entities.SampleEmployees.Dtos;
using Sai.DealAssistant.Domain.Entities.Samples;
using Sai.DealAssistant.Domain.Repositories.Generic;

namespace Sai.DealAssistant.Application.Entities.SampleEmployees.Queries
{
	public class GetSampleEmployeeQuery : IRequest<SampleEmployeeDto>
	{
		public GetSampleEmployeeQuery(int id)
		{
			Id = id;
		}

		public int Id { get; }

		public class Handler : IRequestHandler<GetSampleEmployeeQuery, SampleEmployeeDto>
		{
			private readonly IReadRepository<SampleEmployee> _repository;
			private readonly IMapper _mapper;

			public Handler(IReadRepository<SampleEmployee> repository, IMapper mapper)
			{
				_repository = repository;
				_mapper = mapper;
			}

			public async Task<SampleEmployeeDto> Handle(GetSampleEmployeeQuery query, CancellationToken cancellationToken)
			{
				SampleEmployee? employee = await _repository.FirstOrDefaultAsync(p => p.Id == query.Id);
				if (employee == null)
				{
					throw new NotFoundException(nameof(employee), query.Id);
				}

				return _mapper.Map<SampleEmployeeDto>(employee);
			}
		}
	}
}
