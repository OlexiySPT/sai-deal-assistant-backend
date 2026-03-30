using Sai.DealAssistant.Domain.Entities.ReadOnly.Enums;
using System.Collections.ObjectModel;

namespace Sai.DealAssistant.Domain.Entities;

public class Deal : BaseEntity
{
    public DateOnly StartDate { get; set; }
    public string Name { get; set; } = string.Empty;
    public string Company { get; set; } = string.Empty;
    public string? Description { get; set; } = string.Empty;
    public string? Url { get; set; } = string.Empty;
    public string? AiSearchInfo { get; set; } = string.Empty;
    public string? AiBriefDescription { get; set; } = string.Empty;
    public string? Industry { get; set; } = string.Empty;
    public string? Status { get; set; } = string.Empty;

    public int? FirmId { get; set; }
    public int TypeId { get; set; } = 1;
    public int StateId { get; set; } = 1;


    public decimal? ProposalAmount { get; set; }
    public decimal? MinClientAmount { get; set; }
    public decimal? MaxClientAmount { get; set; }
    public string? CurrencyCode { get; set; }
    public decimal? ExchangeRateToEur { get; set; }
    public int? AmountTypeId { get; set; } = 1;

    public DateTime? DenormLastActionDate { get; set; }


    #region Navigation
    public DealType Type { get; set; } = default!;
    public DealState State { get; set; } = default!;
    public AmountType AmountType { get; set; } = default!;
    public Firm? Firm { get; set; } = default!;

    public ICollection<ContactPerson> ContactPersons { get; set; }
        = new Collection<ContactPerson>();

    public ICollection<Event> Events { get; set; }
        = new Collection<Event>();

    public ICollection<DealTag> Tags { get; set; }
        = new Collection<DealTag>();

    #endregion
}