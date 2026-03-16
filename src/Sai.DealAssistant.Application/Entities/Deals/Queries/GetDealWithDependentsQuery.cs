using AutoMapper;
using MediatR;
using Sai.DealAssistant.Application.Common.Exceptions;
using Sai.DealAssistant.Application.Entities.SampleCustomers.Dtos;
using Sai.DealAssistant.Domain.Entities;
using Sai.DealAssistant.Domain.Repositories;

namespace Sai.DealAssistant.Application.Entities.SampleCustomers.Queries;

public class GetDealWithDependentsQuery : IRequest<DealWithDependentsDto>
{
    public GetDealWithDependentsQuery(int id)
	{
		Id = id;
	}

	public int Id { get; }

	public class Handler : IRequestHandler<GetDealWithDependentsQuery, DealWithDependentsDto>
    {
        private readonly IFullDealRepository _repository;
        private readonly IMapper _mapper;

        public Handler(
            IFullDealRepository repository,
			IMapper mapper)
		{
			_repository = repository;
			_mapper = mapper;
		}

		public async Task<DealWithDependentsDto> Handle(GetDealWithDependentsQuery request, CancellationToken cancellationToken)
		{
            var entity = await _repository.FirstOrDefaultAsync(p => p.Id == request.Id);
            if (entity == null)
            {
                throw new NotFoundExceptionOverride(nameof(Deal), request.Id);
            }
            entity.Tags = [.. entity.Tags.OrderBy(p => p.Id)];

            return _mapper.Map<DealWithDependentsDto>(entity);
        }
	}
}
