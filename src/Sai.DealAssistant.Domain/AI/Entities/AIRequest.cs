namespace Sai.DealAssistant.Domain.Entities;

public class AiRequest
{
    public int Id { get; set; }
    public string Type { get; set; } = default!;
    public string Model { get; set; } = default!;
    public string Prompt { get; set; } = default!;
    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
    public int? DealId { get; set; }
}
