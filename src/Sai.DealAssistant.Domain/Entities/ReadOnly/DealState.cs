using System.Collections.ObjectModel;

namespace Sai.DealAssistant.Domain.Entities.ReadOnly;
//Seeded data entity
public class DealState : BaseReadOnlyEntity
{
    public string State { get; set; } = string.Empty;

    public ICollection<Deal> Deals { get; set; }
        = new Collection<Deal>();
}