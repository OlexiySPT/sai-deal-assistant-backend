using System;
using System.Linq.Expressions;
using System.Reflection;

namespace Sai.DealAssistant.Application.Common.Expressions;

public static class StringSearchExpressions
{
    /// <summary>
    /// Builds an expression: e => selector(e) != null && selector(e).ToLower().Contains(search.ToLowerInvariant())
    /// Provider-translatable (EF Core will translate ToLower + Contains to SQL).
    /// Returns an expression that always 'true' when <paramref name="search"/> is null/empty.
    /// </summary>    
    public static Expression<Func<T, bool>> CaseInsensitiveContains<T>(
        Expression<Func<T, string?>> selector, string? search
    ) 
    where T : class
    {
        return CaseInsensitiveStrMethodCall(
            selector,
            search,
            typeof(string).GetMethod(nameof(string.Contains), new[] { typeof(string) })!
        );
    }

    /// <summary>
    /// Builds an expression: e => selector(e) != null && selector(e).ToLower().StartsWith(search.ToLowerInvariant())
    /// Provider-translatable (EF Core will translate ToLower + StartsWith to SQL).
    /// Returns an expression that always 'true' when <paramref name="search"/> is null/empty.
    /// </summary>    
    public static Expression<Func<T, bool>> CaseInsensitiveStartsWith<T>(
        Expression<Func<T, string?>> selector, string? search
    )
    where T : class
    {
        return CaseInsensitiveStrMethodCall(
            selector,
            search,
            typeof(string).GetMethod(nameof(string.StartsWith), new[] { typeof(string) })!
        );
    }
    public static Expression<Func<T, bool>> CaseInsensitiveEndsWith<T>(
        Expression<Func<T, string?>> selector, string? search
    )
    where T : class
    {
        return CaseInsensitiveStrMethodCall(
            selector,
            search,
            typeof(string).GetMethod(nameof(string.EndsWith), new[] { typeof(string) })!
        );
    }
    public static Expression<Func<T, bool>> CaseInsensitiveEquals<T>(
        Expression<Func<T, string?>> selector, string? search
    )
    where T : class
    {
        return CaseInsensitiveStrMethodCall(
            selector,
            search,
            typeof(string).GetMethod(nameof(string.Equals), new[] { typeof(string) })!
        );
    }

    #region Helpers

    private static Expression<Func<T, bool>> CaseInsensitiveStrMethodCall<T>(
    Expression<Func<T, string?>> selector,
    string? search, MethodInfo method)
    where T : class
    {
        if (string.IsNullOrWhiteSpace(search))
        {
            // No filter
            return _ => true;
        }

        // parameter for the final lambda
        var parameter = selector.Parameters[0];

        // get selector body with the same parameter (in case it came from a different lambda)
        var selectorBody = ReplaceParameter(selector.Body, selector.Parameters[0], parameter);

        // call selectorBody.ToLower()
        var toLower = typeof(string).GetMethod(nameof(string.ToLower), Array.Empty<Type>())!;
        var loweredSelector = Expression.Call(selectorBody, toLower);

        // constant: search.ToLowerInvariant()
        var pattern = Expression.Constant(search.ToLowerInvariant(), typeof(string));

        // call loweredSelector.method(pattern)
        var methodCall = Expression.Call(loweredSelector, method, pattern);

        // ensure null-check: selectorBody != null && containsCall
        var nullCheck = Expression.NotEqual(selectorBody, Expression.Constant(null, typeof(string)));
        var body = Expression.AndAlso(nullCheck, methodCall);

        return Expression.Lambda<Func<T, bool>>(body, parameter);
    }

    private static Expression ReplaceParameter(Expression expression, ParameterExpression from, ParameterExpression to)
        => new ParameterReplaceVisitor(from, to).Visit(expression)!;

    private sealed class ParameterReplaceVisitor : ExpressionVisitor
    {
        private readonly ParameterExpression _from;
        private readonly ParameterExpression _to;

        public ParameterReplaceVisitor(ParameterExpression from, ParameterExpression to)
        {
            _from = from;
            _to = to;
        }

        protected override Expression VisitParameter(ParameterExpression node)
            => node == _from ? _to : base.VisitParameter(node);
    } 
    #endregion
}