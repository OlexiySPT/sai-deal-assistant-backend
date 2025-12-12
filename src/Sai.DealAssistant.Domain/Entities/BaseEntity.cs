using System.ComponentModel.DataAnnotations;

namespace Sai.DealAssistant.Domain.Entities;

public class BaseEntity
{
	public int Id { get; set; }
	public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
    public int CreatedBy { get; set; }
    public DateTime UpdatedAt { get; set; } = DateTime.UtcNow;
    public int UpdatedBy { get; set; }
}
