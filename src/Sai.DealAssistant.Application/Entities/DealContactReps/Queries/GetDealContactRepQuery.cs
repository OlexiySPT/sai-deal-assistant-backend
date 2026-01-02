using AutoMapper;
using MediatR;
using Sai.DealAssistant.Application.Common.Exceptions;
using Sai.DealAssistant.Application.Entities.DealContactReps.Dtos;
using Sai.DealAssistant.Domain.Entities;
using Sai.DealAssistant.Domain.Repositories.Generic;

namespace Sai.DealAssistant.Application.Entities.DealContactReps.Queries;

public class GetDealContactRepQuery : IRequest<DealContactRepDto>
{
    public GetDealContactRepQuery(int id) => Id = id;
    public int Id { get; }

    public class Handler : IRequestHandler<GetDealContactRepQuery, DealContactRepDto>
    {
        private readonly IReadRepository<DealContactRep> _repository;
        private readonly IMapper _mapper;

        public Handler(IReadRepository<DealContactRep> repository, IMapper mapper)
        {
            _repository = repository;
            _mapper = mapper;
        }

        public async Task<DealContactRepDto> Handle(GetDealContactRepQuery request, CancellationToken cancellationToken)
        {
            var entity = await _repository.FirstOrDefaultAsync(p => p.Id == request.Id);
            if (entity == null)
            {
                throw new NotFoundExceptionOverride(nameof(DealContactRep), request.Id);
            }

            return _mapper.Map<DealContactRepDto>(entity);
        }
    }
}