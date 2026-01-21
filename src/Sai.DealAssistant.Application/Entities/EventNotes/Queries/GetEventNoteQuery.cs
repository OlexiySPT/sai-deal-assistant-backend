// Sai.DealAssistant.Application\Entities\EventNotes\Queries\GetEventNoteQuery.cs
using AutoMapper;
using MediatR;
using Sai.DealAssistant.Application.Common.Exceptions;
using Sai.DealAssistant.Application.Entities.EventNotes.Dto;
using Sai.DealAssistant.Domain.Entities;
using Sai.DealAssistant.Domain.Repositories.Generic;

namespace Sai.DealAssistant.Application.Entities.EventNotes.Queries;

public class GetEventNoteQuery : IRequest<EventNoteDto>
{
    public GetEventNoteQuery(int id) => Id = id;
    public int Id { get; }

    public class Handler : IRequestHandler<GetEventNoteQuery, EventNoteDto>
    {
        private readonly IReadRepository<EventNote> _repository;
        private readonly IMapper _mapper;

        public Handler(IReadRepository<EventNote> repository, IMapper mapper)
        {
            _repository = repository;
            _mapper = mapper;
        }

        public async Task<EventNoteDto> Handle(GetEventNoteQuery request, CancellationToken cancellationToken)
        {
            var entity = await _repository.FirstOrDefaultAsync(p => p.Id == request.Id);
            if (entity == null)
            {
                throw new NotFoundExceptionOverride(nameof(EventNote), request.Id);
            }

            return _mapper.Map<EventNoteDto>(entity);
        }
    }
}