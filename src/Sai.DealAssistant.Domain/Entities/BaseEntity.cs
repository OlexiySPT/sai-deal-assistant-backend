using Sai.DealAssistant.Domain.Entities.ReadOnly;

namespace Sai.DealAssistant.Domain.Entities;

public abstract class BaseEntity: BaseReadOnlyEntity
{
	public DateTimeOffset CreatedAt { get; set; } = DateTime.UtcNow;
    public int CreatedBy { get; set; }
    public DateTimeOffset UpdatedAt { get; set; } = DateTime.UtcNow;
    public int UpdatedBy { get; set; }
}
