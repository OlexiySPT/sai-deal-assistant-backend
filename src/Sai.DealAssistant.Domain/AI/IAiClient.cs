namespace Sai.DealAssistant.Domain.AI;

public interface IAiClient
{
    Task<string> Chat(
        AiTaskTypesEnum taskType,
        string prompt,
        int? dealId = null,
        TimeSpan? timeout = null);
}

