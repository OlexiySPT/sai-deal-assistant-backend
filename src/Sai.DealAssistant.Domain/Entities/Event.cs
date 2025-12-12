using Sai.DealAssistant.Domain.Entities.ReadOnly;
using System;
using System.Collections.ObjectModel;
using System.Reflection;

namespace Sai.DealAssistant.Domain.Entities;

public class Event : BaseEntity
{
    public DateTime Date { get; set; }
    public string Status { get; set; } = string.Empty;
    public string? Agenda { get; set; }
    public string? Result { get; set; }

    public int TypeId { get; set; }
    public EventType Type { get; set; } = default!;

    public int ContragentId { get; set; }
    public Contragent Contragent { get; set; } = default!;

    #region Navigation
    public ICollection<EventNote> Notes { get; set; }
        = new Collection<EventNote>();

    public ICollection<EventTag> Tags { get; set; }
        = new Collection<EventTag>();
    #endregion
}
