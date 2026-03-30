using AutoMapper;
using FluentValidation;
using MediatR;
using Sai.DealAssistant.Application.Common.Exceptions;
using Sai.DealAssistant.Application.Entities.Firms.Dtos;
using Sai.DealAssistant.Domain.Entities;
using Sai.DealAssistant.Domain.Repositories.Generic;

namespace Sai.DealAssistant.Application.Entities.Firms.Commands;

public class CreateFirmCommand : FirmDto, IRequest<FirmDto>
{
    public class Validator : AbstractValidator<CreateFirmCommand>
    {
        public Validator()
        {
            RuleFor(c => c.Name)
                .NotEmpty()
                .WithMessage("Name must be provided.")
                .MaximumLength(100)
                .WithMessage("Name must not exceed 100 characters.");

            RuleFor(c => c.Country)
                .NotEmpty()
                .WithMessage("Country must be provided.")
                .MaximumLength(100)
                .WithMessage("Country must not exceed 100 characters.");
        }
    }

    public class Handler : IRequestHandler<CreateFirmCommand, FirmDto>
    {
        private readonly ICrudRepository<Firm> _repository;
        private readonly IMapper _mapper;

        public Handler(ICrudRepository<Firm> repository, IMapper mapper)
        {
            _repository = repository;
            _mapper = mapper;
        }

        public async Task<FirmDto> Handle(CreateFirmCommand request, CancellationToken cancellationToken)
        {
            var newFirm = _mapper.Map<Firm>(request);
            var created = await _repository.CreateAsync(newFirm);

            if (created == null)
            {
                throw new BadRequestExceptionOverride("Failed to create Firm.");
            }

            return _mapper.Map<FirmDto>(created);
        }

        public class MappingProfile : Profile
        {
            public MappingProfile()
            {
                CreateMap<CreateFirmCommand, Firm>();
            }
        }
    }
}