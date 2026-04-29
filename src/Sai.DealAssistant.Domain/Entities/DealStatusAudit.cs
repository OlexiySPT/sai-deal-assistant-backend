namespace Sai.DealAssistant.Domain.Entities;

public class DealStatusAudit : BaseNonTrackedEntity
{
    public int Id { get; set; }
    public int DealId { get; set; }
    public string PreviousValue { get; set; } = string.Empty;
    public DateTimeOffset ChangeDate { get; set; }
    public int ChangeUserId { get; set; }
}
