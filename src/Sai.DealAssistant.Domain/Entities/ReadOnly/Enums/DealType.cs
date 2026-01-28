using System.Collections.ObjectModel;

namespace Sai.DealAssistant.Domain.Entities.ReadOnly.Enums;

public class DealType : BaseNonTrackedEntity, IEnum
{
    public string Type { get; set; } = null!;

    public ICollection<Deal> Deals { get; set; }
        = new Collection<Deal>();
}