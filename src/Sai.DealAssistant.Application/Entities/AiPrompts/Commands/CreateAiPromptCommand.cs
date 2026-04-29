using AutoMapper;
using FluentValidation;
using MediatR;
using Sai.DealAssistant.Application.Common.Exceptions;
using Sai.DealAssistant.Application.Entities.AiPrompts.Dtos;
using Sai.DealAssistant.Domain.Entities;
using Sai.DealAssistant.Domain.Repositories.Generic;
using System.Text.RegularExpressions;

namespace Sai.DealAssistant.Application.Entities.AiPrompts.Commands;

public class CreateAiPromptCommand : AiPromptDto, IRequest<AiPromptDto>
{
    public class Validator : AbstractValidator<CreateAiPromptCommand>
    {
        private readonly IReadRepository<AiPrompt> _promptRepository;

        public Validator(IReadRepository<AiPrompt> promptRepository)
        {
            _promptRepository = promptRepository;

            RuleFor(c => c.Key)
                .NotEmpty()
                .WithMessage("Key must be provided.")
                .MaximumLength(200)
                .WithMessage("Key must not exceed 200 characters.")
                .MustAsync(async (cmd, key, ct) =>
                    !await _promptRepository.ExistsAsync(p => p.Key.ToLower() == key.ToLower() && p.Version == cmd.Version))
                .WithMessage(cmd => $"Prompt with key '{cmd.Key}' and version '{cmd.Version}' already exists.");

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

    public class Handler : IRequestHandler<CreateAiPromptCommand, AiPromptDto>
    {
        private readonly ICrudRepository<AiPrompt> _repository;
        private readonly IMapper _mapper;

        public Handler(ICrudRepository<AiPrompt> repository, IMapper mapper)
        {
            _repository = repository;
            _mapper = mapper;
        }

        public async Task<AiPromptDto> Handle(CreateAiPromptCommand request, CancellationToken cancellationToken)
        {
            var newPrompt = _mapper.Map<AiPrompt>(request);
            var created = await _repository.CreateAsync(newPrompt);

            if (created == null)
            {
                throw new BadRequestExceptionOverride("Failed to create AiPrompt.");
            }

            return _mapper.Map<AiPromptDto>(created);
        }

        public class MappingProfile : Profile
        {
            public MappingProfile()
            {
                CreateMap<CreateAiPromptCommand, AiPrompt>();
            }
        }
    }
}
