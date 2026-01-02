using Sai.DealAssistant.Domain.Entities.ReadOnly.Enums;
using System.Collections.ObjectModel;

namespace Sai.DealAssistant.Domain.Entities;

public class Event : BaseEntity
{
    public DateTimeOffset Date { get; set; }
    public int Pos { get; set; }
    public string? Agenda { get; set; }
    public string? Result { get; set; }

    public int TypeId { get; set; }
    public EventType Type { get; set; } = default!;

    /// <summary>
    /// Event mustn't be reassigned to another Deal
    /// so, update must be prohibited on the EF level
    /// </summary>
    public int DealId { get; set; }
    public Deal Deal { get; set; } = default!;

    public int StateId { get; set; } = 1;
    public EventState State { get; set; } = default!;

    #region Navigation
    public ICollection<EventNote> Notes { get; set; }
        = new Collection<EventNote>();

    public ICollection<EventTag> Tags { get; set; }
        = new Collection<EventTag>();
    #endregion
}
