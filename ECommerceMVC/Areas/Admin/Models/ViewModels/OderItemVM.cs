namespace ECommerceMVC.Areas.Admin.Models.ViewModels
{
    public class OrderItemVM
    {
        // Thông tin cơ bản
        public int MaHd { get; set; }
        public string HoTen { get; set; } = string.Empty; 
        public string DienThoai { get; set; } = string.Empty; 
        public DateTime NgayDat { get; set; }
        public string DiaChi { get; set; } = string.Empty;
        // Thông tin sản phẩm
        public int SoLuongSanPham { get; set; }        

        // Thông tin thanh toán
        public double TongTien { get; set; }
        public string CachThanhToan { get; set; } = "COD";

        // Trạng thái
        public int MaTrangThai { get; set; } 
        
        public required string TenTrangThai { get; set; }


    }
}
