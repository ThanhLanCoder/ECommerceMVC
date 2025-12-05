namespace ECommerceMVC.Areas.Admin.ViewModels
{
    public class DashboardVM
    {
        public double DoanhThu { get; set; }
        public int TongDonHang { get; set; }
        public int KhachTruyCap { get; set; }
        public int SanPhamBanRa { get; set; }

        public List<DonHangDashboardVM> DonHangChoXN { get; set; }
    }

    public class DonHangDashboardVM
    {
        public int MaHd { get; set; }
        public string HoTen { get; set; }
        public DateTime NgayDat { get; set; }
        public double TongTien { get; set; }
        public string TrangThai { get; set; }
        public string NguoiXuLy { get; set; }
    }
}
