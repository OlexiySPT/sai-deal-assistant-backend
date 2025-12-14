using Sai.DealAssistant.Domain.Entities.ReadOnly;
using System;
using System.Collections.ObjectModel;
using System.Reflection;

namespace Sai.DealAssistant.Domain.Entities;

public class Event : BaseEntity
{
    public DateTimeOffset Date { get; set; }
    public string? Agenda { get; set; }
    public string? Result { get; set; }

    public int TypeId { get; set; }
    public EventType Type { get; set; } = default!;

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
