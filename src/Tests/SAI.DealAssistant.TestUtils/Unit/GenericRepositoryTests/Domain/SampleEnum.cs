namespace Sai.DealAssistant.Domain.Entities.ReadOnly.Enums;

public class SampleEnum : BaseReadOnlyEntity, IEnum
{
    public string State { get; set; } = string.Empty;
}
