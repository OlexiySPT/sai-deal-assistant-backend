using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Caching.Memory;
using Sai.DealAssistant.Domain.Entities;
using Sai.DealAssistant.Domain.Entities.ReadOnly.Enums;
using Sai.DealAssistant.Domain.Repositories.Generic;

namespace Sai.DealAssistant.Infrastructure.Repositories.Generic;

public class EnumCache<TEntity> : IEnumCache<TEntity>
    where TEntity : BaseNonTrackedEntity, IEnum, new()
{
    private static readonly MemoryCache s_cache = new(new MemoryCacheOptions());
    private readonly string _cacheKey = $"{typeof(TEntity).FullName ?? typeof(TEntity).Name}--EnumCache--50464213-40AF-45C8-A8F9-41C89D011950";
    private readonly int _expirationMinutes = 10;
    private readonly IReadRepository<TEntity> _repository;

    // Use repository.GetAll() to load values
    public EnumCache(IReadRepository<TEntity> repository, int expirationMinutes)
    {
        _repository = repository ?? throw new ArgumentNullException(nameof(repository));
        _expirationMinutes = expirationMinutes;
    }

    public async Task<IReadOnlyCollection<TEntity>> GetAllAsync()
    {
        if (s_cache.TryGetValue(_cacheKey, out IReadOnlyCollection<TEntity>? cached) && cached is not null)
        {
            return cached;
        }

        // materialize asynchronously from IQueryable via EF Core
        var array = await _repository.GetAll().OrderBy(p => p.Id).ToArrayAsync();
        var items = Array.AsReadOnly(array);

        var options = new MemoryCacheEntryOptions
        {
            AbsoluteExpirationRelativeToNow = TimeSpan.FromMinutes(_expirationMinutes)
        };

        s_cache.Set(_cacheKey, items, options);
        return items;
    }


    // Optional helper to invalidate cache
    public void Invalidate() => s_cache.Remove(_cacheKey);
}
