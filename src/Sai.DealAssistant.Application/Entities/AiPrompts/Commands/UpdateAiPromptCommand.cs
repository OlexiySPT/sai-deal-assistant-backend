using AutoMapper;
using FluentValidation;
using MediatR;
using Sai.DealAssistant.Application.Common.Exceptions;
using Sai.DealAssistant.Application.Entities.AiPrompts.Dtos;
using Sai.DealAssistant.Domain.Entities;
using Sai.DealAssistant.Domain.Repositories.Generic;
using System.Text.RegularExpressions;

namespace Sai.DealAssistant.Application.Entities.AiPrompts.Commands;

public class UpdateAiPromptCommand : AiPromptDto, IRequest<AiPromptDto>
{
    public class Validator : AbstractValidator<UpdateAiPromptCommand>
    {
        private readonly IReadRepository<AiPrompt> _promptRepository;

        public Validator(IReadRepository<AiPrompt> promptRepository)
        {
            _promptRepository = promptRepository;

            RuleFor(c => c.Id)
                .GreaterThan(0)
                .WithMessage("Id must be greater than 0.");

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

    public class Handler : IRequestHandler<UpdateAiPromptCommand, AiPromptDto>
    {
        private readonly ICrudRepository<AiPrompt> _repository;
        private readonly IMapper _mapper;

        public Handler(ICrudRepository<AiPrompt> repository, IMapper mapper)
        {
            _repository = repository;
            _mapper = mapper;
        }

        public async Task<AiPromptDto> Handle(UpdateAiPromptCommand request, CancellationToken cancellationToken)
        {
            var promptToUpdate = _mapper.Map<AiPrompt>(request);
            var updated = await _repository.UpdateAsync(promptToUpdate);

            if (updated == null)
            {
                throw new NotFoundExceptionOverride(nameof(AiPrompt), request.Id);
            }

            return _mapper.Map<AiPromptDto>(updated);
        }

        public class MappingProfile : Profile
        {
            public MappingProfile()
            {
                CreateMap<UpdateAiPromptCommand, AiPrompt>();
            }
        }
    }
}
