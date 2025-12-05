using AutoMapper;
using ECommerceMVC.Helper;
using ECommerceMVC.Models.Entities;
using ECommerceMVC.Models.ViewModels;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;
using System.Threading.Tasks;

namespace ECommerceMVC.Controllers
{
    public class KhachHangController : Controller
    {
        private readonly EcomerceContext db;
        private readonly IMapper _mapper;

        public KhachHangController(EcomerceContext context, IMapper mapper)
        {
            db = context;
            _mapper = mapper;
        }

        [HttpGet]
        public IActionResult DangKy()
        {
            return View();
        }

        [HttpPost]
        public IActionResult DangKy(DangKyVM model, IFormFile Hinh)
        {
            if (ModelState.IsValid)
            {
                var khachHang = _mapper.Map<KhachHang>(model);
                khachHang.MaKh = "KH" + DateTime.Now.ToString("yyyyMMddHHmmss");
                khachHang.MatKhau = PasswordHasher.HashPassword(model.MatKhau);
                khachHang.HieuLuc = true;
                khachHang.VaiTro = 0; // Vai trò mặc định là khách hàng
                if (Hinh != null)
                {
                    khachHang.Hinh = MyUntil.UploadImage(Hinh, "khachhang");
                }

                db.KhachHangs.Add(khachHang);
                db.SaveChanges();
                return RedirectToAction("Index", "Home");
            }
            return View(model);

        }


        [HttpGet]
        public IActionResult DangNhap()
        {
            return View();
        }

        [HttpPost]
        public async Task<IActionResult> DangNhapAsync(DangNhapVM model)
        {
            if (!ModelState.IsValid)
            {
                return View(model);
            }

            var khachHang = db.KhachHangs
                .FirstOrDefault(kh => kh.Email == model.Email);
            if(khachHang == null)
            {
                ModelState.AddModelError(string.Empty, "Sai tên đăng nhập hoặc mật khẩu ");
                return View(model);
            }
            if(!PasswordHasher.HashPassword(model.MatKhau).Equals(khachHang.MatKhau))
            {
                ModelState.AddModelError(string.Empty, "Sai tên đăng nhập hoặc mật khẩu ");
                return View(model);
            }

            var claims = new List<Claim>
            {
                new Claim(ClaimTypes.Name, khachHang.HoTen),
                new Claim(ClaimTypes.Email, khachHang.Email),
                new Claim("UserId", khachHang.MaKh),
                new Claim(ClaimTypes.Role, khachHang.VaiTro == 1 ? "Admin" : "User")
            };
            var identity = new ClaimsIdentity(claims, CookieAuthenticationDefaults.AuthenticationScheme);
            var principal = new ClaimsPrincipal(identity);

            await HttpContext.SignInAsync(
                CookieAuthenticationDefaults.AuthenticationScheme,
                principal,
            new AuthenticationProperties
            {
                IsPersistent = true, // ghi nhớ đăng nhập
                ExpiresUtc = DateTime.UtcNow.AddDays(7)
            });


            if (khachHang.VaiTro == 1) // Admin
                return RedirectToAction("Index", "Home", new {area = "Admin"});

            return RedirectToAction("Index", "Home");
        }

        [HttpGet]
        public IActionResult Profile()
        {
            // Lấy MaKh từ claim
            string? userId = User.FindFirst("UserId")?.Value;
            if (string.IsNullOrEmpty(userId))
                return RedirectToAction("DangNhap");

            var khachHang = db.KhachHangs.FirstOrDefault(k => k.MaKh == userId);
            if (khachHang == null)
                return RedirectToAction("DangNhap");

            // Lấy thống kê đơn hàng
            var donHangs = db.HoaDons
                             .Where(d => d.MaKh == userId)
                             .ToList();

            var tongDonHang = donHangs.Count;
            var hoanThanh = donHangs.Count(d => d.MaTrangThaiNavigation.TenTrangThai == "Đã Giao Hàng");
            var dangXuLy = donHangs.Count(d => d.MaTrangThaiNavigation.TenTrangThai != "Đã Giao Hàng");

            var model = new ProfileVM
            {
                MaKh = khachHang.MaKh,
                HoTen = khachHang.HoTen,
                Email = khachHang.Email,
                DiaChi = khachHang.DiaChi,
                DienThoai = khachHang.DienThoai ?? "",
                NgaySinh = khachHang.NgaySinh,
                GioiTinh = khachHang.GioiTinh,
                Hinh = khachHang.Hinh,
                TongDonHang = tongDonHang,
                DonHangHoanThanh = hoanThanh,
                DonHangDangXuLy = dangXuLy
            };

            return View(model);
        }

        [Authorize]
        public IActionResult LichSuDonHang()
        {
            var userId = User.FindFirst("UserId")?.Value;

            // Lấy dữ liệu thô từ DB trước
            var hoaDons = db.HoaDons
                .Where(h => h.MaKh == userId)
                .OrderByDescending(h => h.NgayDat)
                .ToList();

            var list = hoaDons.Select(h => new DonHangVM
            {
                MaHd = h.MaHd,
                NgayDat = h.NgayDat,
                TongTien = db.ChiTietHds
                    .Where(ct => ct.MaHd == h.MaHd)
                    .Sum(ct => (ct.DonGia * ct.SoLuong) - ct.GiamGia),

                TrangThai = h.MaTrangThai switch
                {
                    0 => "Chờ xác nhận",
                    1 => "Đã thanh toán",
                    2 => "Chờ giao hàng",
                    3 => "Hoàn thành",
                    -1 => "Đã Hủy",
                    _ => "Unknown"  
                }

            }).ToList();

            return View(list);
        }

        [Authorize]
        public IActionResult HuyDon(int id)
        {
            var userId = User.FindFirst("UserId")?.Value;

            var hd = db.HoaDons.FirstOrDefault(h => h.MaHd == id && h.MaKh == userId);

            if (hd == null)
            {
                TempData["Error"] = "Không tìm thấy đơn hàng!";
                return RedirectToAction("LichSuDonHang");
            }

            if (hd.MaTrangThai != 0)
            {
                TempData["Error"] = "Chỉ có thể hủy đơn đang chờ xác nhận!";
                return RedirectToAction("LichSuDonHang");
            }

            hd.MaTrangThai = -1; // Đơn bị hủy
            db.SaveChanges();

            TempData["Success"] = "Hủy đơn hàng thành công!";
            return RedirectToAction("LichSuDonHang");
        }


        [HttpGet]
        public async Task<IActionResult> DangXuat()
        {
            await HttpContext.SignOutAsync(CookieAuthenticationDefaults.AuthenticationScheme);
            return RedirectToAction("DangNhap", "KhachHang");
        }

     

    }
}
