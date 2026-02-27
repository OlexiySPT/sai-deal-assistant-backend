using MediatR;
using Sai.DealAssistant.Application.Common.Expressions;
using Sai.DealAssistant.Application.Entities.SampleCustomers.Dtos;
using Sai.DealAssistant.Common.Enums;
using Sai.DealAssistant.Domain.Entities;
using Sai.DealAssistant.Domain.Repositories.Generic;
using System.Linq.Expressions;

namespace Sai.DealAssistant.Application.Entities.SampleCustomers.Queries;

public class GetDealListQuery : PagedQueryRequest<QueryResult<DealListItemDto>>
{
	public GetDealListQuery()
		: base()
	{
	}
	public string? Name { get; set; }
	public string? Description { get; set; }
	public string? Industry { get; set; }
    public string? Status { get; set; }
    public int[]? StateIds { get; set; }
    public int[]? TypeIds { get; set; }

    public class Handler : IRequestHandler<GetDealListQuery, QueryResult<DealListItemDto>>
	{
		private readonly IReadRepository<Deal> _repository;

		public Handler(IReadRepository<Deal> repository)
		{
			_repository = repository;
		}

		public async Task<QueryResult<DealListItemDto>> Handle(
			GetDealListQuery request, CancellationToken cancellationToken)
		{
			var qry = _repository.GetAll();
			if (!string.IsNullOrWhiteSpace(request.Name))
            {
                qry = qry.Where(StringSearchExpressions.CaseInsensitiveContains<Deal>(x => x.Name!, request.Name));
            }
            if (!string.IsNullOrWhiteSpace(request.Description))
			{
				qry = qry.Where(StringSearchExpressions.CaseInsensitiveContains<Deal>(x => x.Description!,request.Description));
			}
			if (!string.IsNullOrWhiteSpace(request.Industry))
			{
				qry = qry.Where(StringSearchExpressions.CaseInsensitiveContains<Deal>(x => x.Industry!, request.Industry));
			}
			if (request.StateIds is not null && request.StateIds.Any())
			{
				qry = qry.Where(x => request.StateIds.Contains(x.StateId));
			}
            if (request.TypeIds is not null && request.TypeIds.Any())
            {
                qry = qry.Where(x => request.TypeIds.Contains(x.TypeId));
            }
            if (!string.IsNullOrWhiteSpace( request.Status))
            {
                qry = qry.Where(x => request.Status.StartsWith(x.Status!));
            }

            var totalItems = await _repository.CountAsync(qry);

			var result = await _repository.SelectPageAsync(
				qry,
				p => new DealListItemDto
				{
					Id = p.Id,
					Name = p.Name,
					Description = p.Description,
					Industry = p.Industry,
					State = p.State.State,
					Type = p.Type.Type,
					CreatedAt = p.CreatedAt,
                },
				request.Page,
				request.PageSize,
				string.IsNullOrWhiteSpace(request.SortBy) ? null : request.SortBy.ToLowerInvariant(),
				request.SortDirection == SortDirections.Desc
			);
			return new PagedQueryResult<DealListItemDto>(result, totalItems, request.PageSize, request.Page);
		}
	}
}
