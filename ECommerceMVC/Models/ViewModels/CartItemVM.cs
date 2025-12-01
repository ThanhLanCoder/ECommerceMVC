namespace ECommerceMVC.Models.ViewModels
{
    public class CartItemVM
    {
        public int MaHH { get; set; }
        public required string TenHH { get; set; }
        public double DonGia { get; set; }
        public string? Hinh { get; set; }
        public int SoLuong { get; set; }
        public int SoLuongTon { get; set; } = 10;
        public double ThanhTien => DonGia * SoLuong;
    }
}
