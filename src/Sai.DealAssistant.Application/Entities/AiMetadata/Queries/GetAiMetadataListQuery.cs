using MediatR;
using Sai.DealAssistant.Application.Common.Expressions;
using Sai.DealAssistant.Application.Entities.AiMetadatas.Dtos;
using Sai.DealAssistant.Common.Enums;
using Sai.DealAssistant.Domain.Entities;
using Sai.DealAssistant.Domain.Repositories.Generic;

namespace Sai.DealAssistant.Application.Entities.AiMetadatas.Queries;

public class GetAiMetadataListQuery : PagedQueryRequest<QueryResult<AiMetadataDto>>
{
    public string? Type { get; set; }
    public string? Key { get; set; }

    public class Handler : IRequestHandler<GetAiMetadataListQuery, QueryResult<AiMetadataDto>>
    {
        private readonly IReadRepository<AiMetadata> _repository;

        public Handler(IReadRepository<AiMetadata> repository)
        {
            _repository = repository;
        }

        public async Task<QueryResult<AiMetadataDto>> Handle(
            GetAiMetadataListQuery request, CancellationToken cancellationToken)
        {
            var qry = _repository.GetAll();

            if (!string.IsNullOrWhiteSpace(request.Type))
            {
                qry = qry.Where(StringSearchExpressions.CaseInsensitiveEquals<AiMetadata>(x => x.Type, request.Type));
            }

            if (!string.IsNullOrWhiteSpace(request.Key))
            {
                qry = qry.Where(StringSearchExpressions.CaseInsensitiveContains<AiMetadata>(x => x.Key, request.Key));
            }

            var totalItems = await _repository.CountAsync(qry);

            var result = await _repository.SelectPageAsync(
                qry,
                p => new AiMetadataDto
                {
                    Id = p.Id,
                    Type = p.Type,
                    Key = p.Key,
                    Version = p.Version,
                    Text = p.Text
                },
                request.Page,
                request.PageSize,
                string.IsNullOrWhiteSpace(request.SortBy) ? null : request.SortBy.ToLowerInvariant(),
                request.SortDirection == SortDirections.Desc
            );

            return new PagedQueryResult<AiMetadataDto>(result, totalItems, request.PageSize, request.Page);
        }
    }
}
