using AutoMapper;
using MediatR;
using Sai.DealAssistant.Application.Common.Exceptions;
using Sai.DealAssistant.Application.Entities.Events.Dtos;
using Sai.DealAssistant.Domain.Entities;
using Sai.DealAssistant.Domain.Repositories.Generic;

namespace Sai.DealAssistant.Application.Entities.Events.Queries;

public class GetEventQuery : IRequest<EventDto>
{
    public GetEventQuery(int id)
    {
        Id = id;
    }

    public int Id { get; }

    public class Handler : IRequestHandler<GetEventQuery, EventDto>
    {
        private readonly IReadRepository<Event> _repository;
        private readonly IMapper _mapper;

        public Handler(IReadRepository<Event> repository, IMapper mapper)
        {
            _repository = repository;
            _mapper = mapper;
        }

        public async Task<EventDto> Handle(GetEventQuery request, CancellationToken cancellationToken)
        {
            var entity = await _repository.FirstOrDefaultAsync(p => p.Id == request.Id);
            if (entity == null)
            {
                throw new NotFoundExceptionOverride(nameof(Event), request.Id);
            }

            return _mapper.Map<EventDto>(entity);
        }
    }
}