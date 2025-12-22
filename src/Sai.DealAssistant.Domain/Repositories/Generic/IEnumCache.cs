using Sai.DealAssistant.Domain.Entities.ReadOnly;
using Sai.DealAssistant.Domain.Entities.ReadOnly.Enums;

namespace Sai.DealAssistant.Domain.Repositories.Generic;

public interface IEnumCache<TEntity>
    where TEntity : BaseReadOnlyEntity, IEnum, new()
{
    Task<IReadOnlyCollection<TEntity>> GetAllAsync();
}
