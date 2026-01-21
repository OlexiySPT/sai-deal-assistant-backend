using AutoMapper;
using FluentValidation;
using MediatR;
using Sai.DealAssistant.Application.Common.Exceptions;
using Sai.DealAssistant.Application.Entities.DealTags.Dto;
using Sai.DealAssistant.Domain.Entities;
using Sai.DealAssistant.Domain.Repositories.Generic;

namespace Sai.DealAssistant.Application.Entities.DealTags.Commands;

public class AddDealTagIfNotExistsCommand : DealTagDto, IRequest<DealTagDto>
{

    public class Validator : AbstractValidator<AddDealTagIfNotExistsCommand>
    {
        private readonly IReadRepository<Deal> _dealRepository;

        public Validator(IReadRepository<Deal> dealRepository)
        {
            _dealRepository = dealRepository;

            RuleFor(c => c.DealId)
                .GreaterThan(0)
                .WithMessage("DealId must be greater than 0.")
                .MustAsync(async (cmd, dealId, cToken) => await _dealRepository.FirstOrDefaultAsync(d => d.Id == dealId) != null)
                .WithMessage(cmd => $"Deal with Id {cmd.DealId} was not found.");

            RuleFor(c => c.Tag)
                .NotEmpty()
                .WithMessage("Tag must be provided.");
        }
    }

    public class Handler : IRequestHandler<AddDealTagIfNotExistsCommand, DealTagDto>
    {
        private readonly ICrudRepository<DealTag> _repository;
        private readonly IMapper _mapper;

        public Handler(ICrudRepository<DealTag> repository, IMapper mapper)
        {
            _repository = repository;
            _mapper = mapper;
        }

        public async Task<DealTagDto> Handle(AddDealTagIfNotExistsCommand request, CancellationToken cancellationToken)
        {
            var existingentity = await _repository.FirstOrDefaultAsync(dt => dt.DealId == request.DealId && dt.Tag.ToLower() == request.Tag.ToLower());
            if (existingentity != null)
            {
                return _mapper.Map<DealTagDto>(existingentity);
            }

            var newEntity = _mapper.Map<DealTag>(request);
            DealTag? created = await _repository.CreateAsync(newEntity);

            if (created == null)
            {
                throw new NotFoundExceptionOverride(nameof(DealTag), request.DealId);
            }

            return _mapper.Map<DealTagDto>(created);
        }

        public class MappingProfile : Profile
        {
            public MappingProfile()
            {
                CreateMap<AddDealTagIfNotExistsCommand, DealTag>();
            }
        }
    }
}