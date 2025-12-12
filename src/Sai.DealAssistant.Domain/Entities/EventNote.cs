using System;

namespace Sai.DealAssistant.Domain.Entities;

public class EventNote : BaseEntity
{
    public string Text { get; set; } = string.Empty;

    public int EventId { get; set; }
    public Event Event { get; set; } = default!;
}
