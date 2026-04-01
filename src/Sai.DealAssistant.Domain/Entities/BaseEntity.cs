namespace Sai.DealAssistant.Domain.Entities;

public abstract class BaseEntity: BaseNonTrackedEntity
{
    public Guid GlobalId { get; set; } = Guid.NewGuid();
    public DateTimeOffset CreatedAt { get; set; } = DateTime.UtcNow;
    public int CreatedBy { get; set; }
    public DateTimeOffset UpdatedAt { get; set; } = DateTime.UtcNow;
    public int UpdatedBy { get; set; }
}
