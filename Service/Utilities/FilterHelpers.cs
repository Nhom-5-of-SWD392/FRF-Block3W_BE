using Data.EFCore;

namespace Service.Utilities;

public interface IFilterHelper<T>
{
}

public class FilterHelper<T> : IFilterHelper<T>
{
    private readonly DataContext _dataContext;

    public FilterHelper(DataContext dataContext)
    {
        _dataContext = dataContext;
    }

}
