
using Microsoft.EntityFrameworkCore;
using Sai.DealAssistant.Domain.AI.Repositories;
using Sai.DealAssistant.Domain.Entities;
using Sai.DealAssistant.Infrastructure.Persistence;

namespace Sai.DealAssistant.Infrastructure.AI.Repositories;

public class AiResultRepository : IAiResultRepository
{
    private readonly AppDbContext _dbContext;

    public AiResultRepository(AppDbContext dbContext)
    {
        _dbContext = dbContext;
    }

    public async Task<(AiRequest? request, AiResult? result)> GetRequestAndResult(int id)
    {
        var request = await _dbContext.AIRequests.FirstOrDefaultAsync(r => r.Id == id);
        var result = await _dbContext.AiResults.FirstOrDefaultAsync(r => r.RequestId == id);

        return (request, result);
    }

    public async Task<int> AddRequestAsync(AiRequest aiRequest)
    {
        await _dbContext.AIRequests.AddAsync(aiRequest);
        await _dbContext.SaveChangesAsync();
        return aiRequest.Id;
    }

    public async Task AddResultAsync(AiResult aiResult)
    {
        await _dbContext.AiResults.AddAsync(aiResult);
        await _dbContext.SaveChangesAsync();
    }
}
