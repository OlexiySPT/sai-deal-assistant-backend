using Sai.DealAssistant.Domain.Entities;
using System.Linq.Expressions;

namespace Sai.DealAssistant.Domain.Repositories;

public interface IFullDealRepository
{
    Task<Deal?> FirstOrDefaultAsync(Expression<Func<Deal, bool>> predicate);

    Task<Deal?> SingleOrDefaultAsync(Expression<Func<Deal, bool>> predicate);
}
