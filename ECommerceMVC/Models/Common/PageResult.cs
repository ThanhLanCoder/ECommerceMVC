namespace ECommerceMVC.Models.Common
{
    public class PagedResult<T>
    {
        public List<T> Items { get; set; } = new List<T>();
        public int Page { get; set; }
        public int PageSize { get; set; }
        public int TotalItems { get; set; }
        public int TotalPages { get; set; }

        public int? Loai { get; set; }
        public string? Search { get; set; }
    }
}
