using ECommerceMVC.Areas.Admin.Models.ViewModels;
using ECommerceMVC.Models.Entities;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace ECommerceMVC.Areas.Admin.Controllers
{
    [Area("Admin")]
    public class OrderController : Controller
    {
        private readonly EcomerceContext db;

        public OrderController(EcomerceContext context) {
            db = context;
        }

        public IActionResult Index(string? search, int? status, DateTime? fromDate, DateTime? toDate, string? payment)
        {
            var query = db.HoaDons
                .Include(h => h.MaKhNavigation)
                .Include(h => h.ChiTietHds)
                    .ThenInclude(ct => ct.MaHhNavigation)
                .Include(h => h.MaTrangThaiNavigation) // Quan trọng: include để lấy tên trạng thái
                .AsQueryable();

            if (!string.IsNullOrEmpty(search))
                query = query.Where(h => h.MaHd.ToString().Contains(search) || h.HoTen.Contains(search));

            if (status.HasValue)
                query = query.Where(h => h.MaTrangThai == status.Value);

            if (fromDate.HasValue)
                query = query.Where(h => h.NgayDat >= fromDate.Value);

            if (toDate.HasValue)
            {
                var endDate = toDate.Value.AddDays(1);
                query = query.Where(h => h.NgayDat < endDate);
            }

            if (!string.IsNullOrEmpty(payment))
            {
                if (payment.ToLower() == "cod")
                    query = query.Where(h => h.CachThanhToan == "COD");
                else
                    query = query.Where(h => h.CachThanhToan != "COD");
            }

            var orders = query
                .OrderByDescending(h => h.NgayDat)
                .Select(h => new OrderItemVM
                {
                    MaHd = h.MaHd,
                    HoTen = h.HoTen,
                    DienThoai = h.DienThoai,
                    NgayDat = h.NgayDat,
                    SoLuongSanPham = h.ChiTietHds.Count,
                    TongTien = (double)(h.ChiTietHds.Sum(ct => ct.DonGia * ct.SoLuong) + h.PhiVanChuyen),
                    CachThanhToan = h.CachThanhToan,
                    MaTrangThai = h.MaTrangThai,
                    TenTrangThai = h.MaTrangThaiNavigation.TenTrangThai // Lấy tên trạng thái
                })
                .ToList();

            var Orders = db.HoaDons.ToList();

            var viewModel = new OrderListVM
            {
                Orders = orders,
                SearchTerm = search,
                Status = status,
                FromDate = fromDate,
                ToDate = toDate,
                PaymentMethod = payment,
                PendingCount = Orders.Count(h => h.MaTrangThai == 0),
                ShippingCount = Orders.Count(h => h.MaTrangThai == 2),
                CompletedCount = Orders.Count(h => h.MaTrangThai == 3),
                CancelledCount = Orders.Count(h => h.MaTrangThai == -1),
                TotalOrders = orders.Count
            };

            return View(viewModel);
        }


        public IActionResult Details(int id)
        {
            var order = db.HoaDons
                .Include(h => h.MaKhNavigation)
                .Include(h => h.ChiTietHds)
                    .ThenInclude(ct => ct.MaHhNavigation)
                .Include(h => h.MaTrangThaiNavigation)
                .FirstOrDefault(h => h.MaHd == id);

            if (order == null)
                return NotFound();

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
                Products = order.ChiTietHds.Select(ct => new OrderProductItemVM
                {
                    TenHh = ct.MaHhNavigation?.TenHh ?? "",
                    DonGia = (double)ct.DonGia,
                    SoLuong = ct.SoLuong
                }).ToList()
            };

            return View(vm);
        }

        public IActionResult Print(int id)
        {
            var order = db.HoaDons
                .Include(h => h.MaKhNavigation)
                .Include(h => h.ChiTietHds)
                    .ThenInclude(ct => ct.MaHhNavigation)
                .Include(h => h.MaTrangThaiNavigation)
                .FirstOrDefault(h => h.MaHd == id);

            if (order == null)
                return NotFound();

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
                Products = order.ChiTietHds.Select(ct => new OrderProductItemVM
                {
                    TenHh = ct.MaHhNavigation?.TenHh ?? "",
                    DonGia = (double)ct.DonGia,
                    SoLuong = ct.SoLuong
                }).ToList()
            };

            // Dùng layout in gọn
            return View(vm); // View Print.cshtml
        }

        [HttpPost]
        public IActionResult UpdateStatus(int id, int status)
        {
            var order = db.HoaDons
                .Include(h => h.MaTrangThaiNavigation)
                .FirstOrDefault(h => h.MaHd == id);

            if (order == null)
                return Json(new { success = false, message = "Không tìm thấy đơn hàng" });

            order.MaTrangThai = status;
            db.SaveChanges();

            var tenTrangThai = db.TrangThais
                .FirstOrDefault(t => t.MaTrangThai == status)?.TenTrangThai ?? "";

            return Json(new { success = true, name = tenTrangThai });
        }

    }
}