using Microsoft.AspNetCore.Mvc;
using ECommerceMVC.Models.Entities;
using ECommerceMVC.Areas.Admin.ViewModels;
using System.Linq;

namespace ECommerceMVC.Areas.Admin.Controllers
{
    [Area("Admin")]
    public class HomeController : Controller
    {
        private readonly EcomerceContext db;

        public HomeController(EcomerceContext context)
        {
            db = context;
        }

        public IActionResult Index()
        {

            var doanhThu = db.ChiTietHds
                .Where(ct => ct.MaHdNavigation.MaTrangThai == 1)
                .Sum(ct => (ct.DonGia * ct.SoLuong) - ct.GiamGia);

            var tongDonHang = db.HoaDons.Count();


            var sanPhamBanRa = db.ChiTietHds.Sum(ct => (int?)ct.SoLuong) ?? 0;


            var donHangChoXN = db.HoaDons
                .Where(h => h.MaTrangThai == 0)
                .OrderByDescending(h => h.NgayDat)
                .Take(10)
                .Select(h => new DonHangDashboardVM
                {
                    MaHd = h.MaHd,
                    HoTen = h.HoTen,
                    NgayDat = h.NgayDat,
                    TrangThai = "Chờ xác nhận",
                    TongTien = h.ChiTietHds
                        .Sum(ct => (ct.DonGia * ct.SoLuong) - ct.GiamGia)
                })
                .ToList();

            var vm = new DashboardVM
            {
                DoanhThu = doanhThu,
                TongDonHang = tongDonHang,
                KhachTruyCap = 15240, 
                SanPhamBanRa = sanPhamBanRa,
                DonHangChoXN = donHangChoXN
            };

            return View(vm);
        }
    }
}
