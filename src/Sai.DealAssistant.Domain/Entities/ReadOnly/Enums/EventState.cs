using System.Collections.ObjectModel;

namespace Sai.DealAssistant.Domain.Entities.ReadOnly.Enums;

public class EventState : BaseReadOnlyEntity, IEnum
{
    public string State { get; set; } = string.Empty;

    public ICollection<Event> Events { get; set; }
        = new Collection<Event>();
}
