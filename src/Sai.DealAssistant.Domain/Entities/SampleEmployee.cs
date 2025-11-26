namespace Sai.DealAssistant.Domain.Entities
{
	public class SampleEmployee : BaseEntity
	{
		public int CustomerId { get; set; }

		public string FirstName { get; set; } = string.Empty;

		public string LastName { get; set; } = string.Empty;

		public string? Email { get; set; }

		#region Navigational Props
		public SampleCustomer Customer { get; set; } = null!;
		#endregion

		// This is server computed field, needed for creating index for search
		public string FullName { get; set; } = string.Empty;
	}
}
