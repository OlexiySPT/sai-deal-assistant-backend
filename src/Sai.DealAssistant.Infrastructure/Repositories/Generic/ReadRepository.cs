using Microsoft.EntityFrameworkCore;
using Sai.DealAssistant.Domain.Entities;
using Sai.DealAssistant.Domain.Exceptions;
using Sai.DealAssistant.Domain.Repositories.Generic;
using System.Linq.Expressions;
using System.Reflection;

namespace Sai.DealAssistant.Infrastructure.Repositories.Generic;

public class ReadRepository<TDbContext, TEntity> : IReadRepository<TEntity>
	where TEntity : BaseNonTrackedEntity, new()
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
		bool descending)
	{
		if (column is null) throw new ArgumentNullException(nameof(column));
		if (string.IsNullOrWhiteSpace(column)) throw new ArgumentException("Sort column must be provided.", nameof(column));

		// Find top-level property on TEntity using case-insensitive comparison.
		var prop = typeof(TEntity)
			.GetProperties(BindingFlags.Public | BindingFlags.Instance)
			.FirstOrDefault(p => string.Equals(p.Name, column, StringComparison.OrdinalIgnoreCase));

		if (prop is null)
		{
			// Provide available property names for the exception (top-level properties)
			var available = typeof(TEntity)
				.GetProperties(BindingFlags.Public | BindingFlags.Instance)
				.Select(p => p.Name)
				.OrderBy(n => n);
			throw new InvalidSortColumnException(column, available);
		}

		var param = Expression.Parameter(typeof(TEntity), "e");
		var propertyAccess = Expression.Property(param, prop);

		// Convert value types to object to match Expression<Func<TEntity, object>>
		Expression selectorBody = prop.PropertyType.IsValueType
			? Expression.Convert(propertyAccess, typeof(object))
			: (Expression)propertyAccess;

		var lambda = Expression.Lambda<Func<TEntity, object>>(selectorBody, param);

		return descending
			? query.OrderByDescending(lambda)
			: query.OrderBy(lambda);
	}

	public async Task<int> CountAsync(IQueryable<TEntity> query)
	{
		return await query.CountAsync();
	}

	public async Task<IReadOnlyCollection<TResult>> SelectAsync<TResult>(IQueryable<TEntity> query, Expression<Func<TEntity, TResult>> columns)
	{
		return await query.Select(columns).ToListAsync();
    }
    public async Task<IReadOnlyCollection<TResult>> SelectDistinctAsync<TResult>(IQueryable<TEntity> query, Expression<Func<TEntity, TResult>> columns)
    {
        return await query.Select(columns).Distinct().ToListAsync();
    }

    public async Task<IReadOnlyCollection<TResult>> SelectPageAsync<TResult>(
		IQueryable<TEntity> query,
		Expression<Func<TEntity, TResult>> columns,
		int page,
		int pageSize,
		string? orderByColumn,
		bool orderByDescending)
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
			query = ApplySorting(query, orderByColumn, orderByDescending);
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
