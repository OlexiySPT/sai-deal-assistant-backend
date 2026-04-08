using AutoMapper;
using FluentValidation;
using MediatR;
using Sai.DealAssistant.Application.Common.Exceptions;
using Sai.DealAssistant.Application.Entities.Firms.Dtos;
using Sai.DealAssistant.Domain.Entities;
using Sai.DealAssistant.Domain.Repositories.Generic;

namespace Sai.DealAssistant.Application.Entities.Firms.Commands;

public class UpdateFirmCommand : FirmDto, IRequest<FirmDto>
{
    public class Validator : AbstractValidator<UpdateFirmCommand>
    {
        private readonly IReadRepository<Firm> _firmRepository;

        public Validator(IReadRepository<Firm> firmRepository)
        {
            _firmRepository = firmRepository;
            RuleFor(c => c.Id)
                .GreaterThan(0)
                .WithMessage("Id must be greater than 0.");

            RuleFor(c => c.Name)
                .NotEmpty()
                .WithMessage("Name must be provided.")
                .MaximumLength(100)
                .WithMessage("Name must not exceed 100 characters.")
                .MustAsync(async (cmd, name, cToken) =>
                    !await _firmRepository.ExistsAsync(f => f.Name.ToLower() == name.ToLower()))
                .WithMessage(cmd => $"Firm with name '{cmd.Name}' already exists.");

            RuleFor(c => c.Country)
                .NotEmpty()
                .WithMessage("Country must be provided.")
                .MaximumLength(100)
                .WithMessage("Country must not exceed 100 characters.");
        }
    }

    public class Handler : IRequestHandler<UpdateFirmCommand, FirmDto>
    {
        private readonly ICrudRepository<Firm> _repository;
        private readonly IMapper _mapper;

        public Handler(ICrudRepository<Firm> repository, IMapper mapper)
        {
            _repository = repository;
            _mapper = mapper;
        }

        public async Task<FirmDto> Handle(UpdateFirmCommand request, CancellationToken cancellationToken)
        {
            var firmToUpdate = _mapper.Map<Firm>(request);
            var updated = await _repository.UpdateAsync(firmToUpdate);

            if (updated == null)
            {
                throw new NotFoundExceptionOverride(nameof(Firm), request.Id);
            }

            return _mapper.Map<FirmDto>(updated);
        }

        public class MappingProfile : Profile
        {
            public MappingProfile()
            {
                CreateMap<UpdateFirmCommand, Firm>();
            }
        }
    }
}