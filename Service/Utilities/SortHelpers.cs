using System.Linq.Dynamic.Core;
using System.Reflection;
using System.Text;

namespace Service.Utilities;

public interface ISortHelpers<T>
{
    IQueryable<T> ApplySort(IQueryable<T> entities, string orderByQueryString);
}

public class SortHelper<T> : ISortHelpers<T>
{
    public IQueryable<T> ApplySort(IQueryable<T> entities, string orderByQueryString)
    {
        if (!entities.Any())
            return entities;

        if (string.IsNullOrWhiteSpace(orderByQueryString))
            return entities;

        var orderParams = orderByQueryString.Trim().Split(',');
        var propertyInfos = typeof(T).GetProperties(BindingFlags.Public | BindingFlags.Instance);
        var orderQueryBuilder = new StringBuilder();

        foreach (var param in orderParams)
        {
            if (string.IsNullOrWhiteSpace(param))
                continue;

            var propertyFromQueryName = param.Split(" ")[0];
            var sortingOrder = param.EndsWith(" desc") ? "descending" : "ascending";

            // Check if propertyFromQueryName is a nested property (e.g., "RelatedEntity.PropertyName")
            var propertyParts = propertyFromQueryName.Split('.');
            PropertyInfo? objectProperty = null;
            Type currentType = typeof(T);

            // Traverse each part to reach the final property
            foreach (var part in propertyParts)
            {
                objectProperty = currentType.GetProperty(part, BindingFlags.Public | BindingFlags.Instance | BindingFlags.IgnoreCase);
                if (objectProperty == null)
                {
                    break; // Stop if the property does not exist
                }
                currentType = objectProperty.PropertyType;
            }

            if (objectProperty == null)
                continue;

            orderQueryBuilder.Append($"{propertyFromQueryName} {sortingOrder}, ");
        }

        var orderQuery = orderQueryBuilder.ToString().TrimEnd(',', ' ');

        return string.IsNullOrEmpty(orderQuery) ? entities : entities.OrderBy(orderQuery);
    }

}
