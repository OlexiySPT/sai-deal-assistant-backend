namespace Sai.DealAssistant.Api.Requests.SampleCustomers
{
	public class SearchSampleCustomerRequest : PaginatedRequest
	{
		/// <summary>
		/// Gets or sets Customer Code.
		/// </summary>
		public string? Code { get; set; }

		/// <summary>
		/// Gets or sets Customer's Name.
		/// </summary>
		public string? Name { get; set; }
	}
}
