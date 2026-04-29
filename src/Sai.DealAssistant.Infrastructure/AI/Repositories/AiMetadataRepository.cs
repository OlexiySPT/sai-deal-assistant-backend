using Microsoft.EntityFrameworkCore;
using Sai.DealAssistant.Domain;
using Sai.DealAssistant.Domain.AI.Repositories;
using Sai.DealAssistant.Domain.Entities;
using Sai.DealAssistant.Infrastructure.Persistence;

namespace Sai.DealAssistant.Infrastructure.AI.Repositories;

public class AiMetadataRepository : IAiMetadataRepository
{
    private readonly AppDbContext _dbContext;

    public AiMetadataRepository(AppDbContext dbContext)
    {
        _dbContext = dbContext;
    }

    public async Task<int> CreateAsync(AiMetadata metadata)
    {
        if (!VersionUtil.IsValidVersion(metadata.Version))
            throw new ArgumentException("Version must contain only numbers and dots.", nameof(metadata.Version));

        await _dbContext.Set<AiMetadata>().AddAsync(metadata);
        await _dbContext.SaveChangesAsync();
        return metadata.Id;
    }

    public async Task<int> UpdateAsync(AiMetadata metadata)
    {
        if (!VersionUtil.IsValidVersion(metadata.Version))
            throw new ArgumentException("Version must contain only numbers and dots.", nameof(metadata.Version));

        _dbContext.Set<AiMetadata>().Update(metadata);
        await _dbContext.SaveChangesAsync();
        return metadata.Id;
    }

    public async Task<string?> GetTextAsync(string type, string key, string? version)
    {
        if (string.IsNullOrWhiteSpace(key))
            return null;

        var query = _dbContext.Set<AiMetadata>().Where(p => p.Type == type && p.Key == key);

        if (version != null)
        {
            if (!VersionUtil.IsValidVersion(version))
                throw new ArgumentException("Version must contain only numbers and dots.", nameof(version));

            var exact = await query.FirstOrDefaultAsync(p => p.Version == version);
            return exact?.Text;
        }

        var all = await query.ToListAsync();
        if (all.Count == 0)
            return null;

        var max = all.Aggregate((a, b) => VersionUtil.CompareVersion(a.Version, b.Version) >= 0 ? a : b);
        return max.Text;
    
    }
}
