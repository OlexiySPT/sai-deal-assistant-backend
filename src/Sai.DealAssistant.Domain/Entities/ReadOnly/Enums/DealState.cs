using System.Collections.ObjectModel;

namespace Sai.DealAssistant.Domain.Entities.ReadOnly.Enums;

public class DealState : BaseNonTrackedEntity, IEnum
{
    public string State { get; set; } = null!;

    public ICollection<Deal> Deals { get; set; }
        = new Collection<Deal>();
}