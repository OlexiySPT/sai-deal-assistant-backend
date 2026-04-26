using Sai.DealAssistant.Domain.Entities;

namespace Sai.DealAssistant.Domain.AI.Repositories;

public interface IAiResultRepository
{
    Task<(AiRequest? request, AiResult? result)> GetRequestAndResult(int id);
    Task<int> AddRequestAsync(AiRequest aiRequest);
    Task AddResultAsync(AiResult aiResult);
}
