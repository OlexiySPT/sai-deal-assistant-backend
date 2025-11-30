using Sai.DealAssistant.Common.Enums;

namespace Sai.DealAssistant.Api.Requests
{
	public class PaginatedRequest
	{
		/// <summary>
		/// Gets or sets Sort result by eg. name, default is id.
		/// </summary>
		public string? SortBy { get; set; } = "id";

		/// <summary>
		/// Gets or sets Sort Direction by Ascending or Descending, default is Ascending.
		/// </summary>
		public SortDirections? SortDirection { get; set; } = SortDirections.Asc;

		/// <summary>
		/// Gets or sets Page number of result, default is 1.
		/// </summary>
		public int? Page { get; set; } = 1;

		/// <summary>
		/// Gets or sets and sets Page Size of result, default is 250.
		/// </summary>
		public int? PageSize { get; set; } = 250;
	}
}
