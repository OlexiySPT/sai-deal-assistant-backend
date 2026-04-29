using Sai.DealAssistant.Domain.Entities;

namespace Sai.DealAssistant.Domain.AI.Repositories;

public interface IAiPromptRepository
{
    Task<int> CreateAsync(AiPrompt prompt);
    Task<int> UpdateAsync(AiPrompt prompt);
    Task<string?> GetTextAsync(string key, string? version = null);
}
