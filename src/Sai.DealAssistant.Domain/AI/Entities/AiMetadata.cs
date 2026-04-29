namespace Sai.DealAssistant.Domain.Entities;

public class AiMetadata : BaseNonTrackedEntity
{
    public string Type { get; set; } = default!;
    public string Key { get; set; } = default!;
    public string Version { get; set; } = default!;
    public string Text { get; set; } = default!;
}
