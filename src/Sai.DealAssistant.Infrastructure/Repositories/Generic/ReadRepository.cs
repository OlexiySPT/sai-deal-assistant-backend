using Microsoft.EntityFrameworkCore;
using Sai.DealAssistant.Domain.Entities.ReadOnly;
using Sai.DealAssistant.Domain.Exceptions;
using Sai.DealAssistant.Domain.Repositories.Generic;
using System.Linq.Expressions;

namespace Sai.DealAssistant.Infrastructure.Repositories.Generic;

public class ReadRepository<TDbContext, TEntity> : IReadRepository<TEntity>
	where TEntity : BaseReadOnlyEntity, new()
	where TDbContext : DbContext
{
	public ReadRepository(TDbContext dbContext)
	{
		MyDbContext = dbContext;
		Table = MyDbContext.Set<TEntity>();
	}

	#region Protected Props
	protected TDbContext MyDbContext { get; private set; }

	protected DbSet<TEntity> Table { get; private set; }
	#endregion

	public IQueryable<TEntity> GetAll()
	{
		return Table.AsNoTracking().AsQueryable();
	}

	public IQueryable<TEntity> ApplySorting(
		IQueryable<TEntity> query,
		string? column,
		bool descending,
		Dictionary<string, Expression<Func<TEntity, object>>>? columnsMap)
	{
		if (column is null) throw new ArgumentNullException(nameof(column));
		if (columnsMap is null) throw new ArgumentNullException(nameof(columnsMap));

		if (!columnsMap!.ContainsKey(column!))
		{
			throw new InvalidSortColumnException(column!, columnsMap.Keys);
		}

		if (descending)
		{
			return query.OrderByDescending(columnsMap[column!]);
		}

		return query.OrderBy(columnsMap[column!]);
	}

	public async Task<int> CountAsync(IQueryable<TEntity> query)
	{
		return await query.CountAsync();
	}

	public async Task<IReadOnlyCollection<TResult>> SelectAsync<TResult>(IQueryable<TEntity> query, Expression<Func<TEntity, TResult>> columns)
	{
		return await query.Select(columns).ToListAsync();
	}

	public async Task<IReadOnlyCollection<TResult>> SelectPageAsync<TResult>(
		IQueryable<TEntity> query,
		Expression<Func<TEntity, TResult>> columns,
		int page,
		int pageSize,
		string? orderByColumn,
		bool orderByDescending,
		Dictionary<string, Expression<Func<TEntity, object>>>? orderByColumnsMap)
	{
		if (orderByColumn == null || string.IsNullOrWhiteSpace(orderByColumn))
		{
			if (orderByDescending)
			{
				query = query.OrderByDescending(p => p.Id);
			}
			else
			{
				query = query.OrderBy(p => p.Id);
			}
		}
		else
		{
			query = ApplySorting(query, orderByColumn, orderByDescending, orderByColumnsMap);
		}

		return await query
			.Select(columns)
			.ApplyPaging(page, pageSize)
			.ToListAsync();
	}

	public async Task<bool> ExistsAsync(Expression<Func<TEntity, bool>> predicate)
	{
		return await Table.AnyAsync(predicate);
	}

	public async Task<TEntity?> FirstOrDefaultAsync(Expression<Func<TEntity, bool>> predicate)
	{
		return await Table.FirstOrDefaultAsync(predicate);
	}

	public async Task<TEntity?> SingleOrDefaultAsync(Expression<Func<TEntity, bool>> predicate)
	{
		return await Table.SingleOrDefaultAsync(predicate);
	}
}
