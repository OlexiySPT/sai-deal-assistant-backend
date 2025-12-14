using System.Collections.ObjectModel;

namespace Sai.DealAssistant.Domain.Entities.ReadOnly;
//Seeded data entity
public class EventState : BaseReadOnlyEntity
{
    public string State { get; set; } = string.Empty;

    public ICollection<Event> Events { get; set; }
        = new Collection<Event>();
}
