using MediatR;
using Microsoft.Extensions.Caching.Memory;
using Sai.DealAssistant.Common.Configuration;
using Sai.DealAssistant.Domain.Repositories.Generic;
using System.Linq.Expressions;

namespace Sai.DealAssistant.Application.Entities.General.Queries;

public class GetCachedStringOptions : IRequest<IEnumerable<string>>
{
    public required Type EntityType { get; set; }
    public required string FieldName { get; set; }
    public SortDirection SortDirection { get; set; } = SortDirection.Ascending;

    public class Handler : IRequestHandler<GetCachedStringOptions, IEnumerable<string>>
    {
        private static readonly MemoryCache s_cache = new(new MemoryCacheOptions());
        private static readonly SemaphoreSlim s_lock = new(1, 1);
        private readonly IServiceProvider _serviceProvider;
        private readonly IAppConfiguration _configuration;
        private readonly int _expirationMinutes;

        public Handler(IServiceProvider serviceProvider, IAppConfiguration configuration)
        {
            _serviceProvider = serviceProvider;
            _configuration = configuration;
            _expirationMinutes = configuration?.EnumTablesCacheExpitrationMins ?? 10;
        }

        public async Task<IEnumerable<string>> Handle(GetCachedStringOptions request, CancellationToken cancellationToken)
        {
            if (request.EntityType == null)
                throw new ArgumentException("EntityType must be provided.", nameof(request.EntityType));
            if (string.IsNullOrWhiteSpace(request.FieldName))
                throw new ArgumentException("FieldName must be provided.", nameof(request.FieldName));

            var cacheKey = $"{request.EntityType.Name}_{nameof(GetCachedStringOptions)}_{request.FieldName}";
            if (s_cache.TryGetValue(cacheKey, out IReadOnlyCollection<string>? cached) && cached is not null)
            {
                return cached;
            }

            await s_lock.WaitAsync(cancellationToken);
            try
            {
                if (s_cache.TryGetValue(cacheKey, out cached) && cached is not null)
                {
                    return cached;
                }

                // Resolve repository for the entity type
                var repoType = typeof(IReadRepository<>).MakeGenericType(request.EntityType);
                dynamic repository = _serviceProvider.GetService(repoType) ?? throw new InvalidOperationException($"Repository for {request.EntityType.Name} not found");

                var qry = repository.GetAll();

                // Build lambda: x => x.{FieldName} for string properties
                var param = Expression.Parameter(request.EntityType, "x");
                var property = Expression.PropertyOrField(param, request.FieldName);
                var lambdaType = typeof(Func<,>).MakeGenericType(request.EntityType, typeof(string));
                var lambda = Expression.Lambda(lambdaType, property, param);


                // Call SelectDistinctAsync dynamically with correct parameter types

                var methods = repoType.GetMethods();
                var method = methods.FirstOrDefault(m =>
                    m.Name == "SelectDistinctAsync" &&
                    m.GetParameters().Length == 3 &&
                    m.GetParameters()[0].ParameterType.IsGenericType &&
                    m.GetParameters()[0].ParameterType.GetGenericTypeDefinition() == typeof(IQueryable<>) &&
                    m.GetParameters()[1].ParameterType.IsGenericType &&
                    m.GetParameters()[1].ParameterType.GetGenericTypeDefinition() == typeof(Expression<>)
                );
                if (method == null)
                    throw new InvalidOperationException("SelectDistinctAsync method not found on repository");

                if (method.IsGenericMethodDefinition)
                    method = method.MakeGenericMethod(typeof(string));

                var task = (Task)method.Invoke(repository, new object[] { qry, lambda, request.SortDirection });
                await task.ConfigureAwait(false);
                var resultProperty = task.GetType().GetProperty("Result");
                var items = (IReadOnlyCollection<string>)(resultProperty?.GetValue(task) ?? Array.Empty<string>());
                var orderedItems = items.Where(p=>!string.IsNullOrWhiteSpace(p)).ToArray().AsReadOnly();

                var options = new MemoryCacheEntryOptions
                {
                    AbsoluteExpirationRelativeToNow = TimeSpan.FromMinutes(_expirationMinutes)
                };
                s_cache.Set(cacheKey, orderedItems, options);
                return orderedItems!;
            }
            finally
            {
                s_lock.Release();
            }
        }
        public void InvalidateCache(Type entityType, string fieldName)
        {
            var cacheKey = $"{entityType.Name}_{nameof(GetCachedStringOptions)}_{fieldName}";
            s_cache.Remove(cacheKey);
        }
    }
}
