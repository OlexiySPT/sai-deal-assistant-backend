using AutoMapper;
using FluentValidation;
using MediatR;
using Sai.DealAssistant.Application.Common.Exceptions;
using Sai.DealAssistant.Application.Entities.Events.Dtos;
using Sai.DealAssistant.Domain.Entities;
using Sai.DealAssistant.Domain.Entities.ReadOnly.Enums;
using Sai.DealAssistant.Domain.Repositories.Generic;

namespace Sai.DealAssistant.Application.Entities.Events.Commands;

public class CreateEventCommand : EventDto, IRequest<EventDto>
{
    public int DealId { get; set; }

    public class Validator : AbstractValidator<CreateEventCommand>
    {
        private readonly IEnumCache<EventState> _eventStateCache;
        private readonly IEnumCache<EventType> _eventTypeCache;
        private readonly IReadRepository<Deal> _dealRepository;
        private readonly IReadRepository<ContactPerson> _contactPersonRepository;

        public Validator(
            IEnumCache<EventState> eventStateCache,
            IEnumCache<EventType> eventTypeCache,
            IReadRepository<Deal> dealRepository,
            IReadRepository<ContactPerson> contactPersonRepository)
        {
            _eventStateCache = eventStateCache;
            _eventTypeCache = eventTypeCache;
            _dealRepository = dealRepository;
            _contactPersonRepository = contactPersonRepository;

            RuleFor(c => c.TypeId)
                .MustAsync(async (cmd, typeId, cToken) => (await _eventTypeCache.GetAllAsync()).Any(p => p.Id == typeId))
                .WithMessage($"Incorrect Type Id. It must be one of [{string.Join(", ", _eventTypeCache.GetAllAsync().Result.Select(p => p.Id.ToString()))}]");

            RuleFor(c => c.StateId)
                .MustAsync(async (cmd, stateId, cToken) => (await _eventStateCache.GetAllAsync()).Any(p => p.Id == stateId))
                .WithMessage($"Incorrect State Id. It must be one of [{string.Join(", ", _eventStateCache.GetAllAsync().Result.Select(p => p.Id.ToString()))}]");

            RuleFor(c => c.Date)
                .NotEmpty()
                .WithMessage("Date must be provided.");

            RuleFor(c => c.Topic)
                .NotEmpty()
                .WithMessage("Topic must be provided.");

            // Validate that the Deal exists
            RuleFor(c => c.DealId)
                .MustAsync(async (cmd, dealId, cToken) =>
                    await _dealRepository.FirstOrDefaultAsync(d => d.Id == dealId) != null)
                .WithMessage(cmd => $"Deal with Id {cmd.DealId} was not found.");

            RuleFor(c => c.ContactPersonId)
                .MustAsync(async (cmd, contactPersonId, cToken) =>
                {
                    if (contactPersonId is null)
                    {
                        return true;
                    }

                    // Validate that the Deal and the ContactPerson belong to the same Firm.
                    var deal = await _dealRepository.FirstOrDefaultAsync(d => d.Id == cmd.DealId);
                    if (deal == null || deal.FirmId == 0)
                    {
                        return false;
                    }

                    return await _contactPersonRepository.ExistsAsync(
                        c => c.Id == contactPersonId && c.FirmId == deal.FirmId);
                })
                .WithMessage(cmd => $"Contact Person with Id {cmd.ContactPersonId} does not belong to the same Firm as Deal with Id {cmd.DealId}.");
        }
    }

    public class Handler : IRequestHandler<CreateEventCommand, EventDto>
    {
        private readonly ICrudRepository<Event> _repository;
        private readonly IMapper _mapper;

        public Handler(ICrudRepository<Event> repository, IMapper mapper)
        {
            _repository = repository;
            _mapper = mapper;
        }

        public async Task<EventDto> Handle(CreateEventCommand request, CancellationToken cancellationToken)
        {
            var newEvent = _mapper.Map<Event>(request);
            Event? ev = await _repository.CreateAsync(newEvent);

            if (ev == null)
            {
                throw new NotFoundExceptionOverride(nameof(Event), request.DealId);
            }

            return _mapper.Map<EventDto>(ev);
        }

        public class MappingProfile : Profile
        {
            public MappingProfile()
            {
                CreateMap<CreateEventCommand, Event>()
                    .ForMember(dest => dest.Date, opt => opt.MapFrom(src => src.Date!.ToUniversalTime()));
            }
        }
    }
}