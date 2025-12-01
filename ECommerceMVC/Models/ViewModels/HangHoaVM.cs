namespace ECommerceMVC.Models.ViewModels
{
    public class HangHoaVM
    {
        public int MaHH { get; set; }
        public required string TenHH { get; set; }
        public double DonGia { get; set; }
        public string? Hinh { get; set; }
        public string? MoTaNgan { get; set; }
        public required string TenLoai { get; set; }
    }
}
