namespace Sai.DealAssistant.Domain.Entities;

public class ContactPerson : BaseEntity
{
    public string Name { get; set; } = string.Empty;
    public string? Position { get; set; } = string.Empty;
    public string? Phone { get; set; } = string.Empty;
    public string? Email { get; set; } = string.Empty;
    public string? Description { get; set; } = string.Empty;

    public int FirmId { get; set; }
    public Firm Firm { get; set; } = default!;
}
