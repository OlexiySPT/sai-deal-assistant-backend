using MediatR;
using System.Linq;
using Microsoft.Extensions.Caching.Memory;
using Sai.DealAssistant.Common.Configuration;
using Sai.DealAssistant.Domain.Entities;
using Sai.DealAssistant.Domain.Repositories.Generic;

namespace Sai.DealAssistant.Application.Entities.Deals.Queries;

public class GetCachedDealStatusesQuery : IRequest<IEnumerable<string>>
{
    public class Handler : IRequestHandler<GetCachedDealStatusesQuery, IEnumerable<string>>
    {
        private static readonly MemoryCache s_cache = new(new MemoryCacheOptions());
        private static readonly SemaphoreSlim s_lock = new(1, 1);
        private const string _cacheKey = "GetExistingDealStatusesQuery";
        private readonly int _expirationMinutes = 10;
        private readonly IReadRepository<Deal> _repository;

        public Handler(IReadRepository<Deal> repository, IAppConfiguration configuration)
        {
            _repository = repository;
            _expirationMinutes = configuration?.EnumTablesCacheExpitrationMins??10;
        }

        public async Task<IEnumerable<string>> Handle(GetCachedDealStatusesQuery request, CancellationToken cancellationToken)
        {
            if (s_cache.TryGetValue(_cacheKey, out IReadOnlyCollection<string>? cached) && cached is not null)
            {
                return cached;
            }

            await s_lock.WaitAsync(cancellationToken);
            try
            {
                if (s_cache.TryGetValue(_cacheKey, out cached) && cached is not null)
                {
                    return cached;
                }

                var qry = _repository.GetAll();
                var items = await _repository.SelectDistinctAsync(qry, p => p.Status);
                var orderedItems = items.OrderBy(p => p).ToArray().AsReadOnly();

                var options = new MemoryCacheEntryOptions
                {
                    AbsoluteExpirationRelativeToNow = TimeSpan.FromMinutes(_expirationMinutes)
                };
                s_cache.Set(_cacheKey, orderedItems, options);
                return orderedItems!;
            }
            finally
            {
                s_lock.Release();
            }
        }
        public void InvalidateCache()
        {
            s_cache.Remove(_cacheKey);
        }
    }
}