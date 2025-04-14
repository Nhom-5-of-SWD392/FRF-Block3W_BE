
namespace Data.Models
{
    public abstract class QueryStringParameters
    {
        const int maxPageSize = 50;
        private int _pageIndex { get; set; } = 1;
        public int PageIndex
        {
            get
            {
                return _pageIndex;
            }
            set
            {
                _pageIndex = (value > 0) ? value : 1;
            }
        }
        private int _pageSize = 10;
        public int PageSize { get => _pageSize; set => _pageSize = (value > maxPageSize) ? maxPageSize : value; }
        public string? OrderBy { get; set; }
    }

    public class PagingModel<T>
    {
        public int PageIndex { get; set; }
        public int TotalPages { get; set; }
        public int PageSize { get; set; }
        public int TotalCount { get; set; }
        public bool HasPrevious => PageIndex > 1;
        public bool HasNext => PageIndex < TotalPages;
        public List<T>? pagingData { get; set; }
    }

}
