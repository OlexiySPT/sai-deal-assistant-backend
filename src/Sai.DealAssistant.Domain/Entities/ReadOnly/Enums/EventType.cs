using System.Collections.ObjectModel;

namespace Sai.DealAssistant.Domain.Entities.ReadOnly.Enums;

public class EventType : BaseReadOnlyEntity, IEnum
{
    public string Name { get; set; } = string.Empty;
    public ICollection<Event> Events { get; set; }
        = new Collection<Event>();
}
