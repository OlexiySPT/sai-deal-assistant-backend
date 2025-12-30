using AutoMapper;
using MediatR;
using Sai.DealAssistant.Application.Common.Exceptions;
using Sai.DealAssistant.Application.Entities.SampleCustomers.Dtos;
using Sai.DealAssistant.Domain.Entities;
using Sai.DealAssistant.Domain.Repositories.Generic;

namespace Sai.DealAssistant.Application.Entities.SampleCustomers.Queries;

public class GetDealQuery : IRequest<DealDto>
{
	public GetDealQuery(int id)
	{
		Id = id;
	}

	public int Id { get; }

	public class Handler : IRequestHandler<GetDealQuery, DealDto>
    {
        private readonly IReadRepository<Deal> _repository;
        private readonly IMapper _mapper;

        public Handler(
            IReadRepository<Deal> repository,
			IMapper mapper)
		{
			_repository = repository;
			_mapper = mapper;
		}

		public async Task<DealDto> Handle(GetDealQuery request, CancellationToken cancellationToken)
		{
            var entity = await _repository.FirstOrDefaultAsync(p => p.Id == request.Id);
            if (entity == null)
            {
                throw new NotFoundExceptionOverride(nameof(Deal), request.Id);
            }

            return _mapper.Map<DealDto>(entity);
        }
	}
}
