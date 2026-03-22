using AutoMapper;
using FluentValidation;
using MediatR;
using Sai.DealAssistant.Application.Common.Exceptions;
using Sai.DealAssistant.Application.Entities.SampleCustomers.Dtos;
using Sai.DealAssistant.Domain.Entities;
using Sai.DealAssistant.Domain.Entities.ReadOnly.Enums;
using Sai.DealAssistant.Domain.Repositories.Generic;

namespace Sai.DealAssistant.Application.Entities.Deals.Commands;

public class UpdateDealCommand : DealDto, IRequest<DealDto>
{
    public class Validator : AbstractValidator<UpdateDealCommand>
    {
        private readonly IEnumCache<DealState> _dealStateCache;
        private readonly IEnumCache<DealType> _dealTypeCache;

        public Validator(IEnumCache<DealState> dealStateCache, IEnumCache<DealType> dealTypeCache)
        {
            _dealStateCache = dealStateCache;
            _dealTypeCache = dealTypeCache;

            RuleFor(c => c.Id)
                .GreaterThan(0)
                .WithMessage("Id must be greater than 0.");

            RuleFor(c => c.TypeId)
                .MustAsync(async (cmd, typeId, cToken) => (await _dealTypeCache.GetAllAsync()).Any(p => p.Id == typeId))
                .WithMessage($"Incorrect Type Id. It must be one of [{string.Join(", ", _dealTypeCache.GetAllAsync().Result.Select(p => p.Id.ToString()))}]");

            RuleFor(c => c.Name)
                .NotEmpty()
                .NotNull();

            RuleFor(c => c.Company)
                .NotEmpty()
                .NotNull();

            RuleFor(c => c.StateId)
                .MustAsync(async (cmd, stateId, cToken) => (await _dealStateCache.GetAllAsync()).Any(p => p.Id == stateId))
                .WithMessage($"Incorrect State Id. It must be one of [{string.Join(", ", _dealStateCache.GetAllAsync().Result.Select(p => p.Id.ToString()))}]");

            RuleFor(c => c.Url)
                .Must(url => string.IsNullOrWhiteSpace(url)
                    || (Uri.TryCreate(url, UriKind.Absolute, out var u) && (u.Scheme == Uri.UriSchemeHttp || u.Scheme == Uri.UriSchemeHttps)))
                .WithMessage("Url must be empty or a valid absolute http(s) URL.");
        }
    }

    public class Handler : IRequestHandler<UpdateDealCommand, DealDto>
    {
        private readonly ICrudRepository<Deal> _repository;
        private readonly IMapper _mapper;

        public Handler(ICrudRepository<Deal> repository, IMapper mapper)
        {
            _repository = repository;
            _mapper = mapper;
        }

        public async Task<DealDto> Handle(UpdateDealCommand request, CancellationToken cancellationToken)
        {
            var dealToUpdate = _mapper.Map<Deal>(request);

            var updatedDeal = await _repository.UpdateAsync(dealToUpdate);

            if (updatedDeal == null)
            {
                throw new NotFoundExceptionOverride(nameof(Deal), request.Id);
            }

            return _mapper.Map<DealDto>(updatedDeal);
        }
    }
    public class MappingProfile : Profile
    {
        public MappingProfile()
        {
            CreateMap<UpdateDealCommand, Deal>().ReverseMap();
        }
    }
}

