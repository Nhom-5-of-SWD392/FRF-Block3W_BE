using Data.Entities;
using Microsoft.EntityFrameworkCore;
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

        var toLowerMethod = typeof(string).GetMethod("ToLower", Type.EmptyTypes);
        var containsMethod = typeof(string).GetMethod("Contains", new[] { typeof(string) });

        var propertyToLower = Expression.Call(property, toLowerMethod!);
        var keywordConstant = Expression.Constant(keyword.Trim().ToLower());
        var containsExpression = Expression.Call(propertyToLower, containsMethod, keywordConstant);

        var lambda = Expression.Lambda<Func<T, bool>>(containsExpression, parameter);
        return query.Where(lambda);
    }

    public static IQueryable<Post> SearchIncludingTopics(this IQueryable<Post> query, string keyword)
    {
        if (string.IsNullOrWhiteSpace(keyword))
            return query;

        var lowerKeyword = keyword.Trim().ToLower();

        return query.Where(p =>
            p.Title.ToLower().Contains(lowerKeyword) ||
            (p.PostBy != null && (
                p.PostBy.FirstName.ToLower().Contains(lowerKeyword) ||
                p.PostBy.LastName.ToLower().Contains(lowerKeyword)
            )) ||
            p.PostTopic.Any(pt =>
                pt.Topic != null &&
                pt.Topic.Name.ToLower().Contains(lowerKeyword)
            )
        );
    }

	public static IQueryable<Post> SearchByTitleOrIngredient(this IQueryable<Post> query, string keyword)
	{
		if (string.IsNullOrWhiteSpace(keyword))
			return query;

		var lowerKeyword = keyword.Trim().ToLower();

		return query.Where(p =>
			p.Title.ToLower().Contains(lowerKeyword) ||
			p.PostIngredients!.Any(pi =>
				pi.Ingredient != null &&
				pi.Ingredient.Name.ToLower().Contains(lowerKeyword)
			)
		);
	}
}

