using MediatR;
using Sai.DealAssistant.Common.Enums;

namespace Sai.DealAssistant.Application
{
	public abstract class PagedQueryRequest<TRequest> : IRequest<TRequest>
	{
		public string? SortBy { get; set; }

		public SortDirections SortDirection { get; set; } = SortDirections.Asc;

        public int Page { get; set; } = 1;

        public int PageSize { get; set; } = 10;

        public bool SortDescending => SortDirection == SortDirections.Desc;
	}
}
