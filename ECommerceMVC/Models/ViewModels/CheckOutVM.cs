namespace ECommerceMVC.Models.ViewModels
{
    public class CheckOutVM
    {
        public required string HoTen { get; set; }
        public required string SoDienThoai { get; set; }
        public required string DiaChi { get; set; }
        public string ? GhiChu { get; set; }

        public required string PhuongThucThanhToan { get; set; }
    }
}
