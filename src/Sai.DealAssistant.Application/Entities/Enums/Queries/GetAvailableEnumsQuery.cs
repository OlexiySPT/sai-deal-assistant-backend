using MediatR;
using Sai.DealAssistant.Domain.Entities;
using Sai.DealAssistant.Domain.Entities.ReadOnly.Enums;

namespace Sai.DealAssistant.Application.Entities.Enums.Queries;

public class GetAvailableEnumsQuery : IRequest<IEnumerable<string>>
{
    public class Handler : IRequestHandler<GetAvailableEnumsQuery, IEnumerable<string>>
    {
        public Task<IEnumerable<string>> Handle(GetAvailableEnumsQuery request, CancellationToken cancellationToken)
        {
            var domainAssembly = typeof(BaseNonTrackedEntity).Assembly;

            var enumTypes = domainAssembly
                .GetTypes()
                .Where(t => t.IsClass && !t.IsAbstract
                    && typeof(IEnum).IsAssignableFrom(t)
                    && typeof(BaseNonTrackedEntity).IsAssignableFrom(t))
                .Select(t => t.Name.ToLowerInvariant())
                .OrderBy(n => n)
                .ToArray();

            return Task.FromResult<IEnumerable<string>>(enumTypes);
        }
    }
}