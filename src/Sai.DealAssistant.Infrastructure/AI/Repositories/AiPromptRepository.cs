using Microsoft.EntityFrameworkCore;
using Sai.DealAssistant.Domain;
using Sai.DealAssistant.Domain.AI.Repositories;
using Sai.DealAssistant.Domain.Entities;
using Sai.DealAssistant.Infrastructure.Persistence;
using System;
using System.Text.RegularExpressions;

namespace Sai.DealAssistant.Infrastructure.AI.Repositories;

public class AiPromptRepository : IAiPromptRepository
{
    private readonly AppDbContext _dbContext;

    public AiPromptRepository(AppDbContext dbContext)
    {
        _dbContext = dbContext;
    }

    public async Task<int> CreateAsync(AiPrompt prompt)
    {
        if (!Regex.IsMatch(prompt.Version, @"^[0-9]+(\.[0-9]+)*$"))
            throw new ArgumentException("Version must contain only numbers and dots.", nameof(prompt.Version));

        await _dbContext.Set<AiPrompt>().AddAsync(prompt);
        await _dbContext.SaveChangesAsync();
        return prompt.Id;
    }

    public async Task<int> UpdateAsync(AiPrompt prompt)
    {
        if (!VersionUtil.IsValidVersion(prompt.Version))
            throw new ArgumentException("Version must contain only numbers and dots.", nameof(prompt.Version));

        _dbContext.Set<AiPrompt>().Update(prompt);
        await _dbContext.SaveChangesAsync();
        return prompt.Id;
    }

    public async Task<string?> GetTextAsync(string key, string? version)
    {
        if (string.IsNullOrWhiteSpace(key))
            return null;

        var query = _dbContext.Set<AiPrompt>().Where(p => p.Key == key);

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
