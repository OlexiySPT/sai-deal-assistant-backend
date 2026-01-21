using System.Linq.Expressions;

namespace Sai.DealAssistant.Infrastructure;

public static class QueryableExtensions
{
    public static IQueryable<T> ApplyPaging<T>(this IQueryable<T> source, int page, int pageSize)
    {
        if (page < 1) throw new ArgumentOutOfRangeException(nameof(page));
        if (pageSize < 1) throw new ArgumentOutOfRangeException(nameof(pageSize));
        return source.Skip((page - 1) * pageSize).Take(pageSize);
    }


    public static IQueryable<T> ApplyOrdering<T>(
        this IQueryable<T> query,
        string? sortBy,
        bool sortDescending,
        IDictionary<string, Expression<Func<T, object>>>? columnsMap)
    {
        if (string.IsNullOrWhiteSpace(sortBy) || columnsMap is null || !columnsMap!.ContainsKey(sortBy))
        {
            // No sorting or invalid column, return as is
            return query;
        }

        var keySelector = columnsMap[sortBy];

        return sortDescending
            ? query.OrderByDescending(keySelector)
            : query.OrderBy(keySelector);
    }
}