using Microsoft.EntityFrameworkCore;
using Sai.DealAssistant.Domain.Entities;
using Sai.DealAssistant.Domain.Repositories.Generic;
using Sai.DealAssistant.Infrastructure.Persistence;

namespace Sai.DealAssistant.Infrastructure.Repositories.Generic;

public class CrudRepository<TEntity> : ReadRepository<TEntity>, ICrudRepository<TEntity>
	where TEntity : BaseEntity, new()
{
	public CrudRepository(AppDbContext dbContext)
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
		if (!await Table.AnyAsync(c => c.Id == entity.Id))
		{
			return null;
		}

		Table.Update(entity);

		await MyDbContext.SaveChangesAsync();

		return await Table.FirstOrDefaultAsync(c => c.Id == entity.Id);
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
