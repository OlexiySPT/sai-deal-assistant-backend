using AutoMapper;
using MediatR;
using Sai.DealAssistant.Application.Common.Exceptions;
using Sai.DealAssistant.Application.Entities.AiPrompts.Dtos;
using Sai.DealAssistant.Domain.Entities;
using Sai.DealAssistant.Domain.Repositories.Generic;

namespace Sai.DealAssistant.Application.Entities.AiPrompts.Queries;

public class GetAiPromptQuery : IRequest<AiPromptDto>
{
    public GetAiPromptQuery(int id)
    {
        Id = id;
    }

    public int Id { get; set; }

    public class Handler : IRequestHandler<GetAiPromptQuery, AiPromptDto>
    {
        private readonly IReadRepository<AiPrompt> _repository;
        private readonly IMapper _mapper;

        public Handler(IReadRepository<AiPrompt> repository, IMapper mapper)
        {
            _repository = repository;
            _mapper = mapper;
        }

        public async Task<AiPromptDto> Handle(GetAiPromptQuery request, CancellationToken cancellationToken)
        {
            var prompt = await _repository.FirstOrDefaultAsync(f => f.Id == request.Id);

            if (prompt == null)
            {
                throw new NotFoundExceptionOverride(nameof(AiPrompt), request.Id);
            }

            return _mapper.Map<AiPromptDto>(prompt);
        }
    }
}
