// Sai.DealAssistant.Application\Entities\EventNotes\Commands\CreateEventNoteCommand.cs
using AutoMapper;
using FluentValidation;
using MediatR;
using Sai.DealAssistant.Application.Common.Exceptions;
using Sai.DealAssistant.Application.Entities.EventNotes.Dto;
using Sai.DealAssistant.Domain.Entities;
using Sai.DealAssistant.Domain.Repositories.Generic;

namespace Sai.DealAssistant.Application.Entities.EventNotes.Commands;

public class CreateEventNoteCommand : EventNoteDto, IRequest<EventNoteDto>
{
    public class Validator : AbstractValidator<CreateEventNoteCommand>
    {
        private readonly IReadRepository<Event> _eventRepository;

        public Validator(IReadRepository<Event> eventRepository)
        {
            _eventRepository = eventRepository;

            RuleFor(c => c.EventId)
                .GreaterThan(0)
                .WithMessage("EventId must be greater than 0.")
                .MustAsync(async (cmd, eventId, cToken) => await _eventRepository.FirstOrDefaultAsync(e => e.Id == eventId) != null)
                .WithMessage(cmd => $"Event with Id {cmd.EventId} was not found.");

            RuleFor(c => c.Text)
                .NotEmpty()
                .WithMessage("Text must be provided.");
        }
    }

    public class Handler : IRequestHandler<CreateEventNoteCommand, EventNoteDto>
    {
        private readonly ICrudRepository<EventNote> _repository;
        private readonly IMapper _mapper;

        public Handler(ICrudRepository<EventNote> repository, IMapper mapper)
        {
            _repository = repository;
            _mapper = mapper;
        }

        public async Task<EventNoteDto> Handle(CreateEventNoteCommand request, CancellationToken cancellationToken)
        {
            var newEntity = _mapper.Map<EventNote>(request);
            EventNote? created = await _repository.CreateAsync(newEntity);

            if (created == null)
            {
                throw new NotFoundExceptionOverride(nameof(EventNote), request.EventId);
            }

            return _mapper.Map<EventNoteDto>(created);
        }

        public class MappingProfile : Profile
        {
            public MappingProfile()
            {
                CreateMap<CreateEventNoteCommand, EventNote>();
            }
        }
    }
}