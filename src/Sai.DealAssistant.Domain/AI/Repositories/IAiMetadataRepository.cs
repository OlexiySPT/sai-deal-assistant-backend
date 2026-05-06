using Sai.DealAssistant.Domain.Entities;

namespace Sai.DealAssistant.Domain.AI.Repositories;

public interface IAiMetadataRepository
{
    Task<int> CreateAsync(AiMetadata metadata);
    Task<int> UpdateAsync(AiMetadata metadata);
    Task<string?> GetTextAsync(string type, string key, string? version = null);
}
