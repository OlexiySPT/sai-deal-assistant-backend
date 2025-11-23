namespace Sai.DealAssistant.Infrastructure
{
    public static class QueryableExtensions
    {
        public static IQueryable<T> ApplyPaging<T>(this IQueryable<T> source, int page, int pageSize)
        {
            if (page < 1) throw new ArgumentOutOfRangeException(nameof(page));
            if (pageSize < 1) throw new ArgumentOutOfRangeException(nameof(pageSize));
            return source.Skip((page - 1) * pageSize).Take(pageSize);
        }
    }
}
