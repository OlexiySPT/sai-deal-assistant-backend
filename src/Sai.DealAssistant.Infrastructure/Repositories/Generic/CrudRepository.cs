using Microsoft.EntityFrameworkCore;
using Sai.DealAssistant.Domain.Entities;
using Sai.DealAssistant.Domain.Repositories.Generic;

namespace Sai.DealAssistant.Infrastructure.Repositories.Generic;

public class CrudRepository<TDbContext, TEntity> : ReadRepository<TDbContext, TEntity>, ICrudRepository<TEntity>
	where TEntity : BaseNonTrackedEntity, new()
	where TDbContext : DbContext
{
	public CrudRepository(TDbContext dbContext)
		: base(dbContext)
	{
	}

	public async Task<TEntity> CreateAsync(TEntity entity)
	{
		Table.Add(entity);
		await MyDbContext.SaveChangesAsync();

		TEntity result = await Table.SingleAsync(p => p.Id == entity.Id);
		return result;
	}

	public async Task<TEntity?> UpdateAsync(TEntity entity)
	{
		var tracked = await Table.FirstOrDefaultAsync(c => c.Id == entity.Id);
		if (tracked == null)
		{
			return null;
		}

		MyDbContext.Entry(tracked).CurrentValues.SetValues(entity);

		await MyDbContext.SaveChangesAsync();

		return tracked;
	}

	public async Task<TEntity?> DeleteAsync(int id)
	{
		TEntity? result = await Table.FirstOrDefaultAsync(p => p.Id == id);
		return await DeleteAsync(result);
	}

	public async Task<TEntity?> DeleteAsync(TEntity? entity)
	{
		if (entity == null)
		{
			return await Task.FromResult<TEntity?>(null);
		}

		Table.Remove(entity);
		await MyDbContext.SaveChangesAsync();
		return entity;
	}
}
