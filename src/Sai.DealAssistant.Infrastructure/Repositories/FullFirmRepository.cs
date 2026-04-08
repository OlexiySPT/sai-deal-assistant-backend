using Microsoft.EntityFrameworkCore;
using Sai.DealAssistant.Domain.Entities;
using Sai.DealAssistant.Domain.Repositories;
using Sai.DealAssistant.Infrastructure.Persistence;
using System.Linq.Expressions;

namespace Sai.DealAssistant.Infrastructure.Repositories;

public class FullFirmRepository : IFullFirmRepository
{
    private readonly AppDbContext _appDbContext;

    public FullFirmRepository(AppDbContext appDbContext)
    {
        _appDbContext = appDbContext is not null ? appDbContext : throw new ArgumentNullException(nameof(appDbContext));
    }

    public Task<Firm?> FirstOrDefaultAsync(Expression<Func<Firm, bool>> predicate)
    {
        return _appDbContext.Firms
            .Include(f => f.ContactPersons)
            .AsNoTracking()
            .FirstOrDefaultAsync(predicate);
    }
}
