namespace ECommerceMVC.Areas.Admin.Models.ViewModels
{
    public class AccountVM
    {
        public required string MaKh { get; set; }           
        public required string HoTen { get; set; }       
        public required string  Email { get; set; }       
        public string ? SoDienThoai { get; set; } 
        public string ? DiaChi { get; set; }
        public string CapDo { get; set; } = "Vàng";
        public bool HieuLuc { get; set; }       
        public string? Hinh { get; set; }
        public DateTime? NgaySinh { get; set; }
        public bool GioiTinh { get; set; }
    }
}
