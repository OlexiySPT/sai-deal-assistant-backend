using AutoMapper;
using FluentValidation;
using MediatR;
using Sai.DealAssistant.Application.Common.Exceptions;
using Sai.DealAssistant.Application.Entities.AiMetadatas.Dtos;
using Sai.DealAssistant.Domain.Entities;
using Sai.DealAssistant.Domain.Repositories.Generic;
using System.Text.RegularExpressions;

namespace Sai.DealAssistant.Application.Entities.AiMetadatas.Commands;

public class CreateAiMetadataCommand : AiMetadataDto, IRequest<AiMetadataDto>
{
    public class Validator : AbstractValidator<CreateAiMetadataCommand>
    {
        private readonly IReadRepository<AiMetadata> _repo;

        public Validator(IReadRepository<AiMetadata> repo)
        {
            _repo = repo;

            RuleFor(c => c.Type)
                .NotEmpty()
                .WithMessage("Type must be provided.");

            RuleFor(c => c.Key)
                .NotEmpty()
                .WithMessage("Key must be provided.")
                .MaximumLength(200)
                .WithMessage("Key must not exceed 200 characters.")
                .MustAsync(async (cmd, key, ct) =>
                    !await _repo.ExistsAsync(p => p.Type.ToLower() == cmd.Type.ToLower() && p.Key.ToLower() == key.ToLower() && p.Version == cmd.Version))
                .WithMessage(cmd => $"Metadata with type '{cmd.Type}', key '{cmd.Key}' and version '{cmd.Version}' already exists.");

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

    public class Handler : IRequestHandler<CreateAiMetadataCommand, AiMetadataDto>
    {
        private readonly ICrudRepository<AiMetadata> _repository;
        private readonly IMapper _mapper;

        public Handler(ICrudRepository<AiMetadata> repository, IMapper mapper)
        {
            _repository = repository;
            _mapper = mapper;
        }

        public async Task<AiMetadataDto> Handle(CreateAiMetadataCommand request, CancellationToken cancellationToken)
        {
            var newItem = _mapper.Map<AiMetadata>(request);
            var created = await _repository.CreateAsync(newItem);

            if (created == null)
            {
                throw new BadRequestExceptionOverride("Failed to create AiMetadata.");
            }

            return _mapper.Map<AiMetadataDto>(created);
        }

        public class MappingProfile : Profile
        {
            public MappingProfile()
            {
                CreateMap<CreateAiMetadataCommand, AiMetadata>();
            }
        }
    }
}
