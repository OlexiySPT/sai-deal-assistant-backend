using Sai.DealAssistant.Domain.Entities;
using Sai.DealAssistant.Domain.Entities.ReadOnly.Enums;

namespace Sai.DealAssistant.Domain.Repositories.Generic;

public interface IEnumCache<TEntity>
    where TEntity : BaseNonTrackedEntity, IEnum, new()
{
    Task<IReadOnlyCollection<TEntity>> GetAllAsync();
}
