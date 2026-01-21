using AutoMapper;
using FluentValidation;
using MediatR;
using Sai.DealAssistant.Application.Common.Exceptions;
using Sai.DealAssistant.Application.Entities.EventNotes.Dto;
using Sai.DealAssistant.Domain.Entities;
using Sai.DealAssistant.Domain.Repositories.Generic;

namespace Sai.DealAssistant.Application.Entities.EventNotes.Commands;

public class UpdateEventNoteCommand : EventNoteDto, IRequest<EventNoteDto>
{
    public class Validator : AbstractValidator<UpdateEventNoteCommand>
    {
        public Validator()
        {
            RuleFor(c => c.Id)
                .GreaterThan(0)
                .WithMessage("Id must be greater than 0.");

            RuleFor(c => c.Text)
                .NotEmpty()
                .WithMessage("Text must be provided.");
        }
    }

    public class Handler : IRequestHandler<UpdateEventNoteCommand, EventNoteDto>
    {
        private readonly ICrudRepository<EventNote> _repository;
        private readonly IMapper _mapper;

        public Handler(ICrudRepository<EventNote> repository, IMapper mapper)
        {
            _repository = repository;
            _mapper = mapper;
        }

        public async Task<EventNoteDto> Handle(UpdateEventNoteCommand request, CancellationToken cancellationToken)
        {
            var toUpdate = _mapper.Map<EventNote>(request);
            var updated = await _repository.UpdateAsync(toUpdate);

            if (updated == null)
            {
                throw new NotFoundExceptionOverride(nameof(EventNote), request.Id);
            }

            return _mapper.Map<EventNoteDto>(updated);
        }

        public class MappingProfile : Profile
        {
            public MappingProfile()
            {
                CreateMap<UpdateEventNoteCommand, EventNote>().ReverseMap();
            }
        }
    }
}