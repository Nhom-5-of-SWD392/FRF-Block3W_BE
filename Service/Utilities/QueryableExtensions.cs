using System.Linq.Expressions;

namespace Service.Utilities;

public static class QueryableExtensions
{
    public static IQueryable<T> SearchByKeyword<T>(this IQueryable<T> query, Expression<Func<T, string>> propertySelector, string keyword)
    {
        if (string.IsNullOrWhiteSpace(keyword))
            return query;

        var parameter = propertySelector.Parameters[0];
        var property = propertySelector.Body; 

        // Biểu thức: property.ToLower().Contains(keyword.ToLower())
        var toLowerMethod = typeof(string).GetMethod("ToLower", Type.EmptyTypes);
        var containsMethod = typeof(string).GetMethod("Contains", new[] { typeof(string) });

        var propertyToLower = Expression.Call(property, toLowerMethod);
        var keywordConstant = Expression.Constant(keyword.Trim().ToLower());
        var containsExpression = Expression.Call(propertyToLower, containsMethod, keywordConstant);

        var lambda = Expression.Lambda<Func<T, bool>>(containsExpression, parameter);
        return query.Where(lambda);
    }
}

