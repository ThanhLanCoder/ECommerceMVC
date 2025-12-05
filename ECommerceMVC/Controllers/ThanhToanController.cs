using Microsoft.AspNetCore.Mvc;
using ECommerceMVC.Models.Entities;
using ECommerceMVC.Models.ViewModels;
using ECommerceMVC.Helpers;
using Microsoft.AspNetCore.Authorization;
using Microsoft.Extensions.Options;
using ECommerceMVC.Helper;

namespace ECommerceMVC.Controllers
{
    [Authorize]
    public class ThanhToanController : Controller
    {
        private readonly EcomerceContext db;
        private readonly VnPayConfig _vnPayConfig;

        public ThanhToanController(EcomerceContext context, IOptions<VnPayConfig> vnpayOptions)
        {
            db = context;
            _vnPayConfig = vnpayOptions.Value;
        }

        private List<CartItemVM> GetCart()
        {
            var cart = HttpContext.Session.GetObject<List<CartItemVM>>("Cart");
            if (cart == null)
            {
                cart = new List<CartItemVM>();
                HttpContext.Session.SetObject("Cart", cart);
            }
            return cart;
        }

        private string GetUserId() => User.FindFirst("UserId")?.Value ?? "";

        private string GetClientIp()
        {
            var ipAddress = HttpContext.Connection.RemoteIpAddress;
            if (ipAddress == null) return "127.0.0.1";

            // Chuyển IPv6 sang IPv4
            if (ipAddress.ToString() == "::1") return "127.0.0.1";
            if (ipAddress.IsIPv4MappedToIPv6) return ipAddress.MapToIPv4().ToString();
            if (ipAddress.AddressFamily == System.Net.Sockets.AddressFamily.InterNetworkV6) return "127.0.0.1";

            return ipAddress.ToString();
        }

        private HoaDon CreateHoaDon(CheckOutVM model, string phuongThucThanhToan, int trangThai = 0)
        {
            return new HoaDon
            {
                MaKh = GetUserId(),
                NgayDat = DateTime.Now,
                HoTen = model.HoTen,
                DiaChi = model.DiaChi,
                CachThanhToan = phuongThucThanhToan,
                CachVanChuyen = "Giao hàng nhanh",
                PhiVanChuyen = 0,
                MaTrangThai = trangThai,
                GhiChu = model.GhiChu
            };
        }

        private void SaveChiTietHoaDon(int maHd, List<CartItemVM> cart)
        {
            foreach (var item in cart)
            {
                db.ChiTietHds.Add(new ChiTietHd
                {
                    MaHd = maHd,
                    MaHh = item.MaHH,
                    SoLuong = item.SoLuong,
                    DonGia = item.DonGia
                });
            }
            db.SaveChanges();
        }

        [HttpGet]
        public IActionResult Index()
        {
            var cart = GetCart();
            if (cart.Count == 0)
                return RedirectToAction("Index", "GioHang");

            return View(cart);
        }

        [HttpPost]
        public IActionResult ThanhToanCod(CheckOutVM model)
        {
            var cart = GetCart();
            if (cart.Count == 0)
                return RedirectToAction("Index", "GioHang");

            var hoaDon = CreateHoaDon(model, "COD");
            db.HoaDons.Add(hoaDon);
            db.SaveChanges();

            SaveChiTietHoaDon(hoaDon.MaHd, cart);

            HttpContext.Session.Remove("Cart");
            TempData["SuccessMessage"] = "Đã thanh toán thành công bằng COD!";

            return RedirectToAction("Index", "HangHoa");
        }

        [HttpPost]
        public IActionResult ThanhToanVnpay(CheckOutVM model)
        {
            var cart = GetCart();
            if (!cart.Any())
                return RedirectToAction("Index", "GioHang");

            var hoaDon = CreateHoaDon(model, "BANK", 0); // Trạng thái 0: Chờ thanh toán
            db.HoaDons.Add(hoaDon);
            db.SaveChanges();

            SaveChiTietHoaDon(hoaDon.MaHd, cart);

            string vnpUrl = VnPayHelper.CreatePaymentUrl(
                _vnPayConfig,
                hoaDon,
                cart.Sum(c => c.ThanhTien),
                GetClientIp()
            );

            return Redirect(vnpUrl);
        }

        [HttpGet]
        public IActionResult ReturnUrl()
        {
            var query = HttpContext.Request.Query
                .ToDictionary(k => k.Key, v => v.Value.ToString());

            if (!query.Any() || !query.ContainsKey("vnp_SecureHash"))
            {
                TempData["ErrorMessage"] = "Dữ liệu trả về không hợp lệ";
                return RedirectToAction("Index", "GioHang");
            }

            if (!VnPayHelper.ValidateSignature(query, _vnPayConfig.HashSecret))
            {
                TempData["ErrorMessage"] = "Chữ ký không hợp lệ";
                return RedirectToAction("Index", "GioHang");
            }

            string vnp_ResponseCode = query["vnp_ResponseCode"];
            string vnp_TxnRef = query["vnp_TxnRef"];

            if (vnp_ResponseCode == "00" && int.TryParse(vnp_TxnRef, out int maHd))
            {
                var hoaDon = db.HoaDons.FirstOrDefault(h => h.MaHd == maHd);
                if (hoaDon != null)
                {
                    hoaDon.MaTrangThai = 1; // Đã thanh toán
                    db.SaveChanges();

                    HttpContext.Session.Remove("Cart");
                    TempData["SuccessMessage"] = "Thanh toán VNPAY thành công!";
                    return RedirectToAction("Index", "HangHoa");
                }
            }

            TempData["ErrorMessage"] = "Thanh toán thất bại!";
            return RedirectToAction("Index", "GioHang");
        }
    }
}