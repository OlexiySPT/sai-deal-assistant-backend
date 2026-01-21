using System.Linq.Expressions;

namespace Sai.DealAssistant.Infrastructure;

public class ColumnsMap<TEntity> : Dictionary<string, Expression<Func<TEntity, object?>>>
{
}
