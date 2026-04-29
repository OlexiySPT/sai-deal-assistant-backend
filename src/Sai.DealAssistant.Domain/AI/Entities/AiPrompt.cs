namespace Sai.DealAssistant.Domain.Entities;

public class AiPrompt : BaseNonTrackedEntity
{
    public string Key { get; set; } = default!;
    public string Version { get; set; } = default!;
    public string Text { get; set; } = default!;
}
