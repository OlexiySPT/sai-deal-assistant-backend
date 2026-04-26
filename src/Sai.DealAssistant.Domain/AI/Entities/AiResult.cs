namespace Sai.DealAssistant.Domain.Entities;

public class AiResult
{
    public int RequestId { get; set; }
    public string Result { get; set; } = default!;
    public double DurationSeconds { get; set; }
    public bool Success { get; set; }
    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
}
