
using ECommerceMVC.Models.Entities;
using ECommerceMVC.Models.ViewModels;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace ECommerceMVC.Controllers
{
    public class OrderTrackingController : Controller
    {
        private readonly EcomerceContext db;

        public OrderTrackingController(EcomerceContext context)
        {
            db = context;
        }

        public IActionResult ViewOrder(int id)
        {
            var order = db.HoaDons
                .Include(h => h.ChiTietHds).ThenInclude(ct => ct.MaHhNavigation)
                .Include(h => h.MaTrangThaiNavigation)
                .FirstOrDefault(h => h.MaHd == id);

            if (order == null)
                return NotFound("Không tìm thấy đơn hàng");

            var vm = new OrderDetailsVM
            {
                MaHd = order.MaHd,
                HoTen = order.HoTen,
                DienThoai = order.DienThoai,
                DiaChi = order.DiaChi,
                NgayDat = order.NgayDat,
                CachThanhToan = order.CachThanhToan,
                MaTrangThai = order.MaTrangThai,
                TenTrangThai = order.MaTrangThaiNavigation?.TenTrangThai ?? "",
                PhiVanChuyen = (double)order.PhiVanChuyen,
                Products = order.ChiTietHds.Select(ct => new CartItemVM
                {
                    TenHH = ct.MaHhNavigation?.TenHh ?? "",
                    DonGia = (double)ct.DonGia,
                    SoLuong = ct.SoLuong
                }).ToList()
            };

            return View(vm);
        }
    }
}
