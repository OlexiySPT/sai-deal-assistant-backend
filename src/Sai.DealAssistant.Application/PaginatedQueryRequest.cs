using MediatR;
using Sai.DealAssistant.Common.Enums;

namespace Sai.DealAssistant.Application
{
	public abstract class PaginatedQueryRequest<TRequest> : IRequest<TRequest>
	{
		public PaginatedQueryRequest(
			string? sortBy = null,
			SortDirections? sortDirections = null,
			int? page = null,
			int? pageSize = null)
		{
			SortBy = sortBy;
			SortDirection = sortDirections ?? SortDirections.Asc;
			Page = page ?? 1;
			PageSize = pageSize ?? 20;
		}

		public string? SortBy { get; set; }

		public SortDirections SortDirection { get; set; }

		public int Page { get; set; }

		public int PageSize { get; set; }

		public bool SortDescending => SortDirection == SortDirections.Desc;
	}
}
