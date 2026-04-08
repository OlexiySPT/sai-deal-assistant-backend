using Sai.DealAssistant.Domain.Entities;
using System.Linq.Expressions;

namespace Sai.DealAssistant.Domain.Repositories;

public interface IFullFirmRepository
{
    Task<Firm?> FirstOrDefaultAsync(Expression<Func<Firm, bool>> predicate);
}
