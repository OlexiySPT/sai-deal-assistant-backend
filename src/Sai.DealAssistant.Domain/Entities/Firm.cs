using System.Collections.ObjectModel;

namespace Sai.DealAssistant.Domain.Entities;

public class Firm : BaseEntity
{
    public string Name { get; set; } = string.Empty;
    public string Country { get; set; } = string.Empty;
    public string? Description { get; set; }

    #region Navigation
    public ICollection<Deal> Deals { get; set; }
        = new Collection<Deal>();

    public ICollection<ContactPerson> ContactPersons { get; set; }
        = new Collection<ContactPerson>();
    #endregion
}