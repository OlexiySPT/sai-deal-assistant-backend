namespace Sai.DealAssistant.Domain.Entities.ReadOnly.Enums;

public class SampleEnum : BaseNonTrackedEntity, IEnum
{
    public string State { get; set; } = string.Empty;
}
