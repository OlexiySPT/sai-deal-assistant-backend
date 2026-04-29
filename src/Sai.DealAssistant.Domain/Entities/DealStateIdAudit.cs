namespace Sai.DealAssistant.Domain.Entities;

public class DealStateIdAudit : BaseNonTrackedEntity
{
    public int Id { get; set; }
    public int DealId { get; set; }
    public int PreviousValue { get; set; }
    public string PreviousText { get; set; } = string.Empty;
    public DateTimeOffset ChangeDate { get; set; }
    public int ChangeUserId { get; set; }
}
