namespace ECommerceMVC.Models.ViewModels
{
    public class PhanTrangHangHoaVM
    {
        public required IEnumerable<HangHoaVM> HangHoas { get; set; }
        public int CurrentPage { get; set; }
        public int TotalPages { get; set; }
        public int PageSize { get; set; }
        public int TotalItems { get; set; }
        public int? MaLoai { get; set; }
        public string? Search { get; set; }
    }
}
