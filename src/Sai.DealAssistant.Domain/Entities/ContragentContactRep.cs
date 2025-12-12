using System;

namespace Sai.DealAssistant.Domain.Entities;

public class ContragentContactRep : BaseEntity
{
    public string Name { get; set; } = string.Empty;
    public string? Position { get; set; } = string.Empty;
    public string? Phone { get; set; } = string.Empty;
    public string? Email { get; set; } = string.Empty;
    public string? Description { get; set; } = string.Empty;

    public int ContragentId { get; set; }
    public Contragent Contragent { get; set; } = default!;
}
