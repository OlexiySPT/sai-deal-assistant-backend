using MediatR;
using Sai.DealAssistant.Application.Entities.SampleCustomers.Dtos;
using Sai.DealAssistant.Common.Enums;
using Sai.DealAssistant.Domain.Entities;
using Sai.DealAssistant.Domain.Entities.ReadOnly.Enums;
using Sai.DealAssistant.Domain.Repositories.Generic;
using System.Linq.Expressions;

namespace Sai.DealAssistant.Application.Entities.SampleCustomers.Queries;

public class GetDealListQuery : PaginatedQueryRequest<QueryResult<DealListItemDto>>
{
	public GetDealListQuery()
		: base()
	{
	}
	public string? Name { get; set; }
	public string? Description { get; set; }
	public string? Industry { get; set; }
	public int? StateId { get; set; }

	public class Handler : IRequestHandler<GetDealListQuery, QueryResult<DealListItemDto>>
	{
		private readonly IReadRepository<Deal> _repository;
		private static readonly Dictionary<string, Expression<Func<Deal, object>>> _sortMapping =
			new Dictionary<string, Expression<Func<Deal, object>>>
			{
				{ nameof(Deal.Name).ToLowerInvariant(), x => x.Name! },
				{ nameof(Deal.Industry).ToLowerInvariant(), x => x.Industry! },
				{ nameof(Deal.CreatedAt).ToLowerInvariant(), x=> x.CreatedAt },
			};

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
				qry = qry.Where(x => x.Name != null && x.Name.Contains(request.Name));
			}
			if (!string.IsNullOrWhiteSpace(request.Description))
			{
				qry = qry.Where(x => x.Description != null && x.Description.Contains(request.Description));
			}
			if (!string.IsNullOrWhiteSpace(request.Industry))
			{
				qry = qry.Where(x => x.Industry != null && x.Industry.Contains(request.Industry));
			}
			if (request.StateId is not null)
			{
				qry = qry.Where(x => x.StateId == request.StateId);
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
					State = p.State.State

				},
				request.Page,
				request.PageSize,
				string.IsNullOrWhiteSpace(request.SortBy) ? null : request.SortBy.ToLowerInvariant(),
				request.SortDirection == SortDirections.Desc,
				_sortMapping
			);
			return new QueryResult<DealListItemDto>(result, totalItems);
		}
	}
}
