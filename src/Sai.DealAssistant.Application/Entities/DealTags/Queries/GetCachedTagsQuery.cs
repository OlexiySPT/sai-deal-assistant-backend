using MediatR;
using Microsoft.Extensions.Caching.Memory;
using Microsoft.Extensions.Configuration;
using Sai.DealAssistant.Common.Configuration;
using Sai.DealAssistant.Domain.Entities;
using Sai.DealAssistant.Domain.Repositories.Generic;

namespace Sai.DealAssistant.Application.Entities.DealTags.Queries;

public class GetCachedTagsQuery : IRequest<IEnumerable<string>>
{
    public class Handler : IRequestHandler<GetCachedTagsQuery, IEnumerable<string>>
    {
        private static readonly MemoryCache s_cache = new(new MemoryCacheOptions());
        private const string _cacheKey = "GetExistingTagsQuery";
        private readonly int _expirationMinutes = 10;
        private readonly IReadRepository<DealTag> _repository;

        public Handler(IReadRepository<DealTag> repository, IAppConfiguration configuration)
        {
            _repository = repository;
            _expirationMinutes = configuration?.EnumTablesCacheExpitrationMins??10;
        }

        public async Task<IEnumerable<string>> Handle(GetCachedTagsQuery request, CancellationToken cancellationToken)
        {
            if (s_cache.TryGetValue(_cacheKey, out IReadOnlyCollection<string>? cached) && cached is not null)
            {
                return cached;
            }
            var qry = _repository.GetAll();
            var items = await _repository.SelectDistinctAsync(qry, p => p.Tag);

            var options = new MemoryCacheEntryOptions
            {
                AbsoluteExpirationRelativeToNow = TimeSpan.FromMinutes(_expirationMinutes)
            };
            s_cache.Set(_cacheKey, items, options);
            return items;
        }
        public void InvalidateCache()
        {
            s_cache.Remove(_cacheKey);
        }
    }
}