using AutoMapper;
using MediatR;
using Sai.DealAssistant.Application.Common.Exceptions;
using Sai.DealAssistant.Application.Entities.EventNotes.Dto;
using Sai.DealAssistant.Domain.Entities;
using Sai.DealAssistant.Domain.Repositories.Generic;

namespace Sai.DealAssistant.Application.Entities.EventNotes.Commands;

public class DeleteEventNoteCommand : IRequest<EventNoteDto>
{
    public DeleteEventNoteCommand(int id) => Id = id;
    public int Id { get; set; }

    public class Handler : IRequestHandler<DeleteEventNoteCommand, EventNoteDto>
    {
        private readonly ICrudRepository<EventNote> _repository;
        private readonly IMapper _mapper;

        public Handler(ICrudRepository<EventNote> repository, IMapper mapper)
        {
            _repository = repository;
            _mapper = mapper;
        }

        public async Task<EventNoteDto> Handle(DeleteEventNoteCommand request, CancellationToken cancellationToken)
        {
            EventNote? deleted = await _repository.DeleteAsync(request.Id);

            if (deleted == null)
            {
                throw new NotFoundExceptionOverride(nameof(EventNote), request.Id);
            }

            return _mapper.Map<EventNoteDto>(deleted);
        }
    }
}