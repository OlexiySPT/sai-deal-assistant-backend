using MediatR;
using Microsoft.Extensions.DependencyInjection;
using Sai.DealAssistant.Application.Common.Exceptions;
using Sai.DealAssistant.Domain.Entities;
using Sai.DealAssistant.Domain.Entities.ReadOnly.Enums;
using Sai.DealAssistant.Domain.Repositories.Generic;
using System.Collections;
using System.Reflection;

namespace Sai.DealAssistant.Application.Entities.Enums.Queries;

public class GetEnumItemsQuery : IRequest<IEnumerable<IDictionary<string, object?>>>
{
    public string EnumName { get; set; } = string.Empty;

    public class Handler : IRequestHandler<GetEnumItemsQuery, IEnumerable<IDictionary<string, object?>>>
    {
        private readonly IServiceProvider _serviceProvider;

        public Handler(IServiceProvider serviceProvider)
        {
            _serviceProvider = serviceProvider ?? throw new ArgumentNullException(nameof(serviceProvider));
        }

        public async Task<IEnumerable<IDictionary<string, object?>>> Handle(GetEnumItemsQuery request, CancellationToken cancellationToken)
        {
            if (string.IsNullOrWhiteSpace(request.EnumName))
                throw new BadRequestExceptionOverride("EnumName must be provided.");

            var domainAssembly = typeof(BaseNonTrackedEntity).Assembly;
            var entityType = domainAssembly
                .GetTypes()
                .FirstOrDefault(t => t.IsClass && !t.IsAbstract
                    && typeof(IEnum).IsAssignableFrom(t)
                    && typeof(BaseNonTrackedEntity).IsAssignableFrom(t)
                    && string.Equals(t.Name, request.EnumName, StringComparison.OrdinalIgnoreCase));

            if (entityType is null)
                throw new NotFoundExceptionOverride("Enum type", request.EnumName);

            using var scope = _serviceProvider.CreateScope();
            var provider = scope.ServiceProvider;

            var cacheServiceType = typeof(IEnumCache<>).MakeGenericType(entityType);
            var cacheService = provider.GetService(cacheServiceType);

            if (cacheService is null)
            {
                throw new InvalidOperationException($"IEnumCache<{entityType.Name}> service is not registered.");
            }

            var cachedItems = await ((dynamic)cacheService).GetAllAsync();

            var enumerable = (cachedItems as IEnumerable) ?? throw new InvalidOperationException("Cached items are not enumerable.");

            var shaped = enumerable
                .Cast<object>()
                .Select(item =>
                {
                    var dict = new Dictionary<string, object?>();
                    var props = item.GetType()
                        .GetProperties(BindingFlags.Public | BindingFlags.Instance)
                        .Where(p => IsSimpleType(p.PropertyType));

                    foreach (var p in props)
                    {
                        dict[p.Name] = p.GetValue(item);
                    }

                    return (IDictionary<string, object?>)dict;
                })
                .ToList();

            return shaped;
        }

        private static bool IsSimpleType(Type type)
        {
            var t = Nullable.GetUnderlyingType(type) ?? type;

            if (t.IsPrimitive) return true;
            if (t.IsEnum) return true;
            if (t == typeof(string)) return true;
            if (t == typeof(decimal)) return true;
            if (t == typeof(DateTime)) return true;
            if (t == typeof(DateTimeOffset)) return true;
            if (t == typeof(TimeSpan)) return true;
            if (t == typeof(Guid)) return true;

            if (t.IsValueType && !t.IsPrimitive && !t.IsEnum)
                return true;

            return false;
        }
    }
}