using MediatR;
using Sai.DealAssistant.Application.Common.Expressions;
using Sai.DealAssistant.Application.Entities.AiPrompts.Dtos;
using Sai.DealAssistant.Common.Enums;
using Sai.DealAssistant.Domain.Entities;
using Sai.DealAssistant.Domain.Repositories.Generic;

namespace Sai.DealAssistant.Application.Entities.AiPrompts.Queries;

public class GetAiPromptListQuery : PagedQueryRequest<QueryResult<AiPromptDto>>
{
    public string? Key { get; set; }

    public class Handler : IRequestHandler<GetAiPromptListQuery, QueryResult<AiPromptDto>>
    {
        private readonly IReadRepository<AiPrompt> _repository;

        public Handler(IReadRepository<AiPrompt> repository)
        {
            _repository = repository;
        }

        public async Task<QueryResult<AiPromptDto>> Handle(
            GetAiPromptListQuery request, CancellationToken cancellationToken)
        {
            var qry = _repository.GetAll();

            if (!string.IsNullOrWhiteSpace(request.Key))
            {
                qry = qry.Where(StringSearchExpressions.CaseInsensitiveContains<AiPrompt>(x => x.Key, request.Key));
            }

            var totalItems = await _repository.CountAsync(qry);

            var result = await _repository.SelectPageAsync(
                qry,
                p => new AiPromptDto
                {
                    Id = p.Id,
                    Key = p.Key,
                    Version = p.Version,
                    Text = p.Text
                },
                request.Page,
                request.PageSize,
                string.IsNullOrWhiteSpace(request.SortBy) ? null : request.SortBy.ToLowerInvariant(),
                request.SortDirection == SortDirections.Desc
            );

            return new PagedQueryResult<AiPromptDto>(result, totalItems, request.PageSize, request.Page);
        }
    }
}
