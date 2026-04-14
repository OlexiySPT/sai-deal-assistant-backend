using MediatR;
using Sai.DealAssistant.Application.Common.Expressions;
using Sai.DealAssistant.Application.Entities.Firms.Dtos;
using Sai.DealAssistant.Common.Enums;
using Sai.DealAssistant.Domain.Entities;
using Sai.DealAssistant.Domain.Repositories.Generic;

namespace Sai.DealAssistant.Application.Entities.Firms.Queries;

public class GetFirmListQuery : PagedQueryRequest<QueryResult<FirmListItemDto>>
{
    public string? Name { get; set; }

    public class Handler : IRequestHandler<GetFirmListQuery, QueryResult<FirmListItemDto>>
    {
        private readonly IReadRepository<Firm> _repository;

        public Handler(IReadRepository<Firm> repository)
        {
            _repository = repository;
        }

        public async Task<QueryResult<FirmListItemDto>> Handle(
            GetFirmListQuery request, CancellationToken cancellationToken)
        {
            var qry = _repository.GetAll();

            if (!string.IsNullOrWhiteSpace(request.Name))
            {
                qry = qry.Where(StringSearchExpressions.CaseInsensitiveContains<Firm>(x => x.Name, request.Name));
            }

            var totalItems = await _repository.CountAsync(qry);

            var result = await _repository.SelectPageAsync(
                qry,
                p => new FirmListItemDto
                {
                    Id = p.Id,
                    Name = p.Name,
                    Country = p.Country,
                },
                request.Page,
                request.PageSize,
                string.IsNullOrWhiteSpace(request.SortBy) ? null : request.SortBy.ToLowerInvariant(),
                request.SortDirection == SortDirections.Desc
            );

            return new PagedQueryResult<FirmListItemDto>(result, totalItems, request.PageSize, request.Page);
        }
    }
}