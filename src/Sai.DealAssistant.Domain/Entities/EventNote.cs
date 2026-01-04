namespace Sai.DealAssistant.Domain.Entities;

public class EventNote : BaseNonTrackedEntity
{
    public int Order { get; set; } = 0;
    public string Text { get; set; } = string.Empty;

    public int EventId { get; set; }
    public Event Event { get; set; } = default!;
}
