using System.Collections.ObjectModel;

namespace Sai.DealAssistant.Domain.Entities.ReadOnly.Enums;

public class DealState : BaseReadOnlyEntity, IEnum
{
    public string State { get; set; }

    public ICollection<Deal> Deals { get; set; }
        = new Collection<Deal>();
}