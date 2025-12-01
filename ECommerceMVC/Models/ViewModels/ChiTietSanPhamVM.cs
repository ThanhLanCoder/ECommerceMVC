namespace ECommerceMVC.Models.ViewModels
{
    public class ChiTietSanPhamVM
    {
        public int MaHH { get; set; }
        public required string TenHH { get; set; }
        public double DonGia { get; set; }
        public string? Hinh { get; set; }
        public string? MoTaNgan { get; set; }
        public required string TenLoai { get; set; }
        public string? MoTaChiTiet { get; set; }
        public int DiemDanhGia { get; set; }
        public int SoLuongTon { get; set; }

    }
}
