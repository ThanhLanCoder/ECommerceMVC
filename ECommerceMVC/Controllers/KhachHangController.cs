using AutoMapper;
using ECommerceMVC.Models.Entities;
using Microsoft.AspNetCore.Mvc;
using ECommerceMVC.Models.ViewModels;
using ECommerceMVC.Helper;
using System.Security.Claims;
using Microsoft.AspNetCore.Authentication.Cookies;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authentication;

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
                new Claim("UsedId", khachHang.MaKh),
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
                return RedirectToAction("Index", "Admin");

            return RedirectToAction("Index", "Home");
        }

        [HttpGet]
        public IActionResult Profile()
        {
            // Lấy MaKh từ claim
            string? userId = User.FindFirst("UsedId")?.Value;
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
            var hoanThanh = donHangs.Count(d => d.MaTrangThaiNavigation.TenTrangThai == "Hoàn thành");
            var dangXuLy = donHangs.Count(d => d.MaTrangThaiNavigation.TenTrangThai != "Hoàn thành");

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

        [HttpGet]
        public async Task<IActionResult> DangXuat()
        {
            await HttpContext.SignOutAsync(CookieAuthenticationDefaults.AuthenticationScheme);
            return RedirectToAction("Index", "HangHoa");
        }

    }
}
