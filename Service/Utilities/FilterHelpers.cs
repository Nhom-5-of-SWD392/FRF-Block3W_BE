using Data.EFCore;
using Data.Entities;
using Data.Enum;

namespace Service.Utilities;

public interface IFilterHelper<T>
{
    IQueryable<T> ApplyFilterRequest(IQueryable<T> entities, Dictionary<string, string> filters);
}

public class FilterHelper<T> : IFilterHelper<T>
{
    private readonly DataContext _dataContext;

    public FilterHelper(DataContext dataContext)
    {
        _dataContext = dataContext;
    }

    public IQueryable<T> ApplyFilterRequest(IQueryable<T> entities, Dictionary<string, string> filters)
    {
        if (typeof(T) == typeof(ModeratorApplication))
        {
            var requests = entities as IQueryable<ModeratorApplication>;

            if (filters.ContainsKey("Status") && Enum.TryParse(filters["Status"], out ApplicationStatus status))
            {
                requests = requests.Where(c => c.Status == status);
            }

            return requests as IQueryable<T>;
        }

        return entities;
    }
}
