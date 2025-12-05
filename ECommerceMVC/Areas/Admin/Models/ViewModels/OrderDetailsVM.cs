namespace ECommerceMVC.Areas.Admin.Models.ViewModels
{
    public class OrderDetailsVM
    {
        // Thông tin đơn hàng
        public int MaHd { get; set; }
        public string HoTen { get; set; } = string.Empty;
        public string DienThoai { get; set; } = string.Empty;
        public string DiaChi { get; set; } = string.Empty;
        public DateTime NgayDat { get; set; }
        public string CachThanhToan { get; set; } = "COD";
        public int MaTrangThai { get; set; }
        public string TenTrangThai { get; set; } = string.Empty;
        public double PhiVanChuyen { get; set; }

        // Danh sách sản phẩm trong đơn
        public List<OrderProductItemVM> Products { get; set; } = new List<OrderProductItemVM>();

        // Tổng tiền
        public double TongTien => Products.Sum(p => p.ThanhTien) + PhiVanChuyen;
    }

    public class OrderProductItemVM
    {
        public string TenHh { get; set; } = string.Empty;
        public double DonGia { get; set; }
        public int SoLuong { get; set; }
        public double ThanhTien => DonGia * SoLuong;
    }
}
