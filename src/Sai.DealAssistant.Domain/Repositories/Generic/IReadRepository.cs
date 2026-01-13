using Sai.DealAssistant.Domain.Entities;
using System.Linq.Expressions;

namespace Sai.DealAssistant.Domain.Repositories.Generic;

/// <summary>
/// Generic Repository for typical read operation from one Entity (Table in the DB)
/// Call GetAll() into IQueriable_TEntity
/// Then apply filters and OrderBy using regular LINQ
/// Your query is ready for SelectAsync, CountAsync, FirstOrDefaultAsync, etc
/// Use SelectAsync to select and matherialize result to TResult type
/// This allows avoid selection of unnecessary columns and early hitting performance
/// (especially, when we need just several columns of dozens).
/// </summary>
/// <typeparam name="TEntity">Entity type, mapped to table in the DB.</typeparam>
public interface IReadRepository<TEntity>
	where TEntity : BaseNonTrackedEntity, new()
{
	/// <summary>
	/// Use GetAll() to start every query.
	/// </summary>
	/// <returns>Select * from TEntity query.</returns>
	IQueryable<TEntity> GetAll();

	/// <summary>
	/// Applies sorting for the query by the column using columnsMap.
	/// </summary>
	/// <param name="query">Source query, with applied wheres.</param>
	/// <param name="column">Field to Order by, if not provided, uses Id.</param>
	/// <param name="descending">default false.</param>
	/// <param name="columnsMap">Dictionary, which maps column to OrderBy Expression.</param>
	/// <returns>Sorted query.</returns>
	IQueryable<TEntity> ApplySorting(
		IQueryable<TEntity> query,
		string? column,
		bool descending);

	/// <summary>
	/// Returns matherialized and mapped according to columns param query result.
	/// </summary>
	/// <typeparam name="TResult">Result Type.</typeparam>
	/// <param name="query">Source query, with applied wheres.</param>
	/// <param name="columns">Expression, which maps entity(TEntity) to DTO(TResult).</param>
	/// <returns>Matherialized already mapped dataset.</returns>
	Task<IReadOnlyCollection<TResult>> SelectAsync<TResult>(
		IQueryable<TEntity> query,
		Expression<Func<TEntity, TResult>> columns);

    /// <summary>
    /// Returns Distinct, matherialized and mapped according to columns param query result.
    /// </summary>
    /// <typeparam name="TResult">Result Type.</typeparam>
    /// <param name="query">Source query, with applied wheres.</param>
    /// <param name="columns">Expression, which maps entity(TEntity) to DTO(TResult).</param>
    /// <returns>Matherialized already mapped dataset.</returns>
    Task<IReadOnlyCollection<TResult>> SelectDistinctAsync<TResult>(
        IQueryable<TEntity> query,
        Expression<Func<TEntity, TResult>> columns);

    /// <summary>
    /// Returns matherialized and mapped according to columns paged query result.
    /// </summary>
    /// <typeparam name="TResult">Result Type. Usually it is DTO, which has less columns then entity.</typeparam>
    /// <param name="query">Source query, with applied wheres.</param>
    /// <param name="columns">Expression, which maps entity(TEntity) to DTO(TResult).</param>
    /// <param name="page">Page number for the paged result.</param>
    /// <param name="pageSize">Page size, default 250.</param>
    /// <param name="orderBy">Field to Order by, if not provided, uses Id.</param>
    /// <param name="orderByDescending">default false.</param>
    /// <param name="orderByColumnsMap">Dictionary, which maps column to OrderBy Expression.</param>
    /// <returns>Matherialized already mapped dataset.</returns>
    Task<IReadOnlyCollection<TResult>> SelectPageAsync<TResult>(
		IQueryable<TEntity> query,
		Expression<Func<TEntity, TResult>> columns,
		int page,
		int pageSize,
		string? orderBy = null,
		bool orderByDescending = false);

	/// <summary>
	/// Counts row number in a query.
	/// </summary>
	/// <param name="query">Source query, with applied wheres.</param>
	/// <returns>Rows count for the query.</returns>
	Task<int> CountAsync(IQueryable<TEntity> query);

	/// <summary>
	/// Checks if row exists.
	/// </summary>
	/// <param name="predicate">Predicate to use.</param>
	/// <returns>boolean saying if the entity exists.</returns>
	Task<bool> ExistsAsync(Expression<Func<TEntity, bool>> predicate);

	/// <summary>
	/// Wrapper for FirstOrDefault.
	/// </summary>
	/// <param name="predicate">Predicate to use.</param>
	/// <returns>Entity or null if it is not found.</returns>
	Task<TEntity?> FirstOrDefaultAsync(Expression<Func<TEntity, bool>> predicate);

	/// <summary>
	/// Wrapper for SingleOrDefault, throws an exception when more than one elements found.
	/// </summary>
	/// <param name="predicate">Predicate to use.</param>
	/// <returns>Entity or null if it is not found.</returns>
	Task<TEntity?> SingleOrDefaultAsync(Expression<Func<TEntity, bool>> predicate);
}
