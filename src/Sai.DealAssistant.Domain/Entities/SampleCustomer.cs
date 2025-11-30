using System.Collections.ObjectModel;

namespace Sai.DealAssistant.Domain.Entities;

public class SampleCustomer : BaseEntity
{
	public string Code { get; set; } = string.Empty;

	public string Name { get; set; } = string.Empty;

	// Just to demonstrate, that usually we need just a part of information for different cases
	#region Other information

	#region Contact info
	public string? Country { get; set; }

	public string? Phone { get; set; }

	public string? Email { get; set; }
    #endregion

    #region TaxInfo
    public string? TaxNumber { get; set; }

    public DateTime? DateRegistered { get; set; }

    #endregion
    #endregion

    #region Navigational Props

    // This is a lookup collection, to define a FK, you must create
    public ICollection<SampleEmployee> Employees { get; set; } = new Collection<SampleEmployee>();
	#endregion

}
