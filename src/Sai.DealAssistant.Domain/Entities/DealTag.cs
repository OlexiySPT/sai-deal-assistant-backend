namespace Sai.DealAssistant.Domain.Entities;

public class DealTag : BaseNonTrackedEntity
{
    public string Tag { get; set; } = string.Empty;
    public int DealId { get; set; }
    public Deal Deal { get; set; } = default!;
}
