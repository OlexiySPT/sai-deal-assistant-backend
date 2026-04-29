using AutoMapper;
using FluentValidation;
using MediatR;
using Sai.DealAssistant.Application.Common.Exceptions;
using Sai.DealAssistant.Application.Entities.AiMetadatas.Dtos;
using Sai.DealAssistant.Domain.Entities;
using Sai.DealAssistant.Domain.Repositories.Generic;
using System.Text.RegularExpressions;

namespace Sai.DealAssistant.Application.Entities.AiMetadatas.Commands;

public class UpdateAiMetadataCommand : AiMetadataDto, IRequest<AiMetadataDto>
{
    public class Validator : AbstractValidator<UpdateAiMetadataCommand>
    {
        public Validator()
        {
            RuleFor(c => c.Id)
                .GreaterThan(0)
                .WithMessage("Id must be greater than 0.");

            RuleFor(c => c.Type)
                .NotEmpty()
                .WithMessage("Type must be provided.");

            RuleFor(c => c.Key)
                .NotEmpty()
                .WithMessage("Key must be provided.")
                .MaximumLength(200)
                .WithMessage("Key must not exceed 200 characters.");

            RuleFor(c => c.Version)
                .NotEmpty()
                .WithMessage("Version must be provided.")
                .Must(v => Regex.IsMatch(v, "^[0-9]+(\\.[0-9]+)*$"))
                .WithMessage("Version must contain only numbers and dots.");

            RuleFor(c => c.Text)
                .NotEmpty()
                .WithMessage("Text must be provided.");
        }
    }

    public class Handler : IRequestHandler<UpdateAiMetadataCommand, AiMetadataDto>
    {
        private readonly ICrudRepository<AiMetadata> _repository;
        private readonly IMapper _mapper;

        public Handler(ICrudRepository<AiMetadata> repository, IMapper mapper)
        {
            _repository = repository;
            _mapper = mapper;
        }

        public async Task<AiMetadataDto> Handle(UpdateAiMetadataCommand request, CancellationToken cancellationToken)
        {
            var toUpdate = _mapper.Map<AiMetadata>(request);
            var updated = await _repository.UpdateAsync(toUpdate);

            if (updated == null)
            {
                throw new NotFoundExceptionOverride(nameof(AiMetadata), request.Id);
            }

            return _mapper.Map<AiMetadataDto>(updated);
        }

        public class MappingProfile : Profile
        {
            public MappingProfile()
            {
                CreateMap<UpdateAiMetadataCommand, AiMetadata>();
            }
        }
    }
}
