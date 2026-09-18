namespace Application.Common.Models
{
    public class PaginatedList<T>
    {
        public IReadOnlyCollection<T>? Items {get; init;}
        public int PageNumber {get; init;}
        public int TotalPages {get; init;}
        public int PageSize {get; init;}
        public int TotalItems {get; init;}
    }
}