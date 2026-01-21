using System.Collections.Generic;

namespace Sai.DealAssistant.Application
{
	public class QueryResult<T>
	{
		public QueryResult(IReadOnlyCollection<T> items, int totalItems)
		{
			Items = items;
			TotalItems = totalItems;
		}

		/// <summary>
		/// Gets total Items from the result.
		/// </summary>
		public int TotalItems { get; }

		/// <summary>
		/// Gets a collection of result based a query.
		/// </summary>
		public IReadOnlyCollection<T> Items { get; }
	}
}
