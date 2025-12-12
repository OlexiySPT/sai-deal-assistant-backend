using System;
using System.Collections.ObjectModel;

namespace Sai.DealAssistant.Domain.Entities;

public class Contragent : BaseEntity
{
    public string Name { get; set; } = string.Empty;
    public string? Url { get; set; } = string.Empty;
    public string? Notes { get; set; } = string.Empty;
    public string? AiSearchInfo { get; set; } = string.Empty;
    public string? AiBriefDescription { get; set; } = string.Empty;
    public string? Industry { get; set; } = string.Empty;
    public string? Address { get; set; } = string.Empty;
    public string? Status { get; set; } = string.Empty;

    public int ResponsibleEmployeeId { get; set; }

    #region Navigation
    public ICollection<ContragentContactRep> ContactReps { get; set; }
        = new Collection<ContragentContactRep>();

    public ICollection<Event> Events { get; set; }
        = new Collection<Event>();

    #endregion
}
