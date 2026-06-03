using System.Linq.Expressions;

namespace Data.Data.Common
{
    public class QueryOptions<T> where T : class
    {
        public int PageNumber { get; set; } = 1;
        public int PageSize { get; set; } = 10;

        public string? SortBy { get; set; }
        public bool IsDescending { get; set; }

        public List<Expression<Func<T, bool>>> Filters { get; set; } = new();
    }
}
