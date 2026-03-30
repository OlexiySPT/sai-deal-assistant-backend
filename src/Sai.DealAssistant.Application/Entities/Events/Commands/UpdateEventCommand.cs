using AutoMapper;
using FluentValidation;
using MediatR;
using Sai.DealAssistant.Application.Common.Exceptions;
using Sai.DealAssistant.Application.Entities.Events.Dtos;
using Sai.DealAssistant.Domain.Entities;
using Sai.DealAssistant.Domain.Entities.ReadOnly.Enums;
using Sai.DealAssistant.Domain.Repositories.Generic;

namespace Sai.DealAssistant.Application.Entities.Events.Commands;

public class UpdateEventCommand : EventDto, IRequest<EventDto>
{
    public class Validator : AbstractValidator<UpdateEventCommand>
    {
        private readonly IEnumCache<EventState> _eventStateCache;
        private readonly IEnumCache<EventType> _eventTypeCache;
        private readonly ICrudRepository<ContactPerson> _contactPersonRepository;
        private readonly ICrudRepository<Event> _eventRepository;

        public Validator(
            IEnumCache<EventState> eventStateCache,
            IEnumCache<EventType> eventTypeCache,
            ICrudRepository<ContactPerson> contactPersonRepository,
            ICrudRepository<Event> eventRepository)
        {
            _eventStateCache = eventStateCache;
            _eventTypeCache = eventTypeCache;
            _contactPersonRepository = contactPersonRepository;
            _eventRepository = eventRepository;

            RuleFor(c => c.Id)
                .GreaterThan(0)
                .WithMessage("Id must be greater than 0.");

            RuleFor(c => c.TypeId)
                .MustAsync(async (cmd, typeId, cToken) => (await _eventTypeCache.GetAllAsync()).Any(p => p.Id == typeId))
                .WithMessage($"Incorrect Type Id. It must be one of [{string.Join(", ", _eventTypeCache.GetAllAsync().Result.Select(p => p.Id.ToString()))}]");

            RuleFor(c => c.StateId)
                .MustAsync(async (cmd, stateId, cToken) => (await _eventStateCache.GetAllAsync()).Any(p => p.Id == stateId))
                .WithMessage($"Incorrect State Id. It must be one of [{string.Join(", ", _eventStateCache.GetAllAsync().Result.Select(p => p.Id.ToString()))}]");

            RuleFor(c => c.ContactPersonId)
                .MustAsync(async (cmd, contactPersonId, cToken) =>
                {
                    if (contactPersonId is null)
                    {
                        return true;
                    }
                    var dealId = _eventRepository.GetAll().Where(c => c.Id == cmd.Id).Select(p => p.DealId).FirstOrDefault();
                    return await _contactPersonRepository.ExistsAsync(c => c.Id == contactPersonId && c.DealId == dealId);
                })
                .WithMessage(cmd => $"Contact Person with Id {cmd.ContactPersonId} was not found for this deal.");


            RuleFor(c => c.Date)
                .NotEmpty()
                .WithMessage("Date must be provided.");
        }
    }

    public class Handler : IRequestHandler<UpdateEventCommand, EventDto>
    {
        private readonly ICrudRepository<Event> _repository;
        private readonly IMapper _mapper;

        public Handler(ICrudRepository<Event> repository, IMapper mapper)
        {
            _repository = repository;
            _mapper = mapper;
        }

        public async Task<EventDto> Handle(UpdateEventCommand request, CancellationToken cancellationToken)
        {
            var eventToUpdate = _mapper.Map<Event>(request);
            var updated = await _repository.UpdateAsync(eventToUpdate);

            if (updated == null)
            {
                throw new NotFoundExceptionOverride(nameof(Event), request.Id);
            }

            return _mapper.Map<EventDto>(updated);
        }

        public class MappingProfile : Profile
        {
            public MappingProfile()
            {
                CreateMap<UpdateEventCommand, Event>()
                    .ForMember(dest => dest.Date, opt => opt.MapFrom(src => src.Date!.ToUniversalTime()));
            }
        }
    }
}