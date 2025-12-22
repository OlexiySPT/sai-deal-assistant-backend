using System.Collections.ObjectModel;

namespace Sai.DealAssistant.Domain.Entities.ReadOnly.Enums;

public class DealType : BaseReadOnlyEntity, IEnum
{
    public string Type { get; set; }

    public ICollection<Deal> Deals { get; set; }
        = new Collection<Deal>();
}