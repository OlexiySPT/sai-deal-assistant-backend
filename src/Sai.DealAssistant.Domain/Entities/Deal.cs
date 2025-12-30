using Sai.DealAssistant.Domain.Entities.ReadOnly.Enums;
using System;
using System.Collections.ObjectModel;

namespace Sai.DealAssistant.Domain.Entities;

public class Deal : BaseEntity
{
    public string Name { get; set; } = string.Empty;
    public string? Description { get; set; } = string.Empty;
    public string? Url { get; set; } = string.Empty;
    public string? AiSearchInfo { get; set; } = string.Empty;
    public string? AiBriefDescription { get; set; } = string.Empty;
    public string? Industry { get; set; } = string.Empty;
    public string? Status { get; set; } = string.Empty;

    public int TypeId { get; set; } = 1;
    public int StateId { get; set; } = 1;

    #region Navigation
    public DealType Type { get; set; } = default!;
    public DealState State { get; set; } = default!;
    public ICollection<DealContactRep> ContactReps { get; set; }
        = new Collection<DealContactRep>();

    public ICollection<Event> Events { get; set; }
        = new Collection<Event>();

    #endregion
}
