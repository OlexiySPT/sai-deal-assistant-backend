namespace Sai.DealAssistant.Domain.Entities;

public abstract class BaseEntity: BaseNonTrackedEntity
{
    public byte? SourceId { get; set; } = 1;
    public DateTimeOffset CreatedAt { get; set; } = DateTime.UtcNow;
    public int CreatedBy { get; set; }
    public DateTimeOffset UpdatedAt { get; set; } = DateTime.UtcNow;
    public int UpdatedBy { get; set; }
}
