using Microsoft.EntityFrameworkCore;
using Sai.DealAssistant.Domain.Entities;
using Sai.DealAssistant.Domain.Repositories;
using Sai.DealAssistant.Infrastructure.Persistence;
using System.Linq.Expressions;

namespace Sai.DealAssistant.Infrastructure.Repositories;

public class FullDealRepository : IFullDealRepository
{
	private readonly AppDbContext _appDbContext;

	public FullDealRepository(AppDbContext appDbContext)
	{
		_appDbContext = appDbContext is not null ? appDbContext : throw new ArgumentNullException(nameof(appDbContext));
	}

    public Task<Deal?> FirstOrDefaultAsync(Expression<Func<Deal, bool>> predicate)
    {
        return GetAllWithDependents()
			.FirstOrDefaultAsync(predicate);
    }

    public Task<Deal?> SingleOrDefaultAsync(Expression<Func<Deal, bool>> predicate)
    {
        return GetAllWithDependents()
            .SingleOrDefaultAsync(predicate);
    }

	public IQueryable<Deal> GetAllWithDependents()
	{
		return _appDbContext.Deals
			.Include(d => d.Type)
			.Include(d => d.State)
			.Include(d => d.ContactPersons)
			.Include(d => d.Events)
                .ThenInclude(e => e.Notes)
				.Include(e => e.Type)
				.Include(e => e.State)
            .Include(d => d.Tags)
			.Include(d => d.Firm)
			.AsNoTracking()
			.AsQueryable();
    }
}
