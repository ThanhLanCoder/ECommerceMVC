using ECommerceMVC.Areas.Admin.Models.ViewModels;

namespace ECommerceMVC.Models.ViewModels
{
    public class OrderDetailsVM
    {
        public int MaHd { get; set; }
        public string HoTen { get; set; } = string.Empty;
        public string DienThoai { get; set; } = string.Empty;
        public string DiaChi { get; set; } = string.Empty;

        public DateTime NgayDat { get; set; }

        public string CachThanhToan { get; set; } = string.Empty;

        public int MaTrangThai { get; set; }
        public string TenTrangThai { get; set; } = string.Empty;

        public double PhiVanChuyen { get; set; }

        // Danh sách sản phẩm trong đơn hàng
        public List<CartItemVM> Products { get; set; } = new();
    }
}
