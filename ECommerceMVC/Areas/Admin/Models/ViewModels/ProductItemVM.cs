namespace ECommerceMVC.Areas.Admin.Models.ViewModels
{
    public class ProductItemVM
    {
        public int MaHH { get; set; }
        public string TenHH { get; set; } = string.Empty;
        public double DonGia { get; set; }
        public string Hinh { get; set; } = string.Empty;
        public string MoTaNgan { get; set; } = string.Empty;
        public string MoTaChiTiet { get; set; } = string.Empty;
        public int SoLuongTon { get; set; } = 10;
        public string TenLoai { get; set; } = string.Empty;

    }
}
