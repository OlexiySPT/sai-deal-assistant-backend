using System.Collections.Generic;

namespace Sai.DealAssistant.Application
{
	public class PagedQueryResult<T> : QueryResult<T>
	{
		public PagedQueryResult(IReadOnlyCollection<T> items, int totalItems, int pageSize, int pageNumber)
			:base(items, totalItems)
		{
			PageSize = pageSize;
			PageNumber = pageNumber;
			TotalPages = (int)Math.Ceiling(totalItems / (double)pageSize);
        }

		public int PageSize { get; }
        public int PageNumber { get; }
		public int TotalPages { get; }
	}
}
