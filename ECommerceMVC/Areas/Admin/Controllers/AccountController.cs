using ECommerceMVC.Areas.Admin.Models.ViewModels;
using ECommerceMVC.Helper;
using ECommerceMVC.Models.Entities;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace ECommerceMVC.Areas.Admin.Controllers
{
    [Area("Admin")]
    public class AccountController : Controller
    {
        private readonly EcomerceContext db;
        private readonly IWebHostEnvironment _env;

        public AccountController(EcomerceContext context, IWebHostEnvironment env)
        {
            db = context;
            _env = env;
        }

        public IActionResult Index(string? search, string? status, string? level)
        {
            // Chỉ lấy khách hàng
            var query = db.KhachHangs
                .Where(u => u.VaiTro == 0)
                .AsQueryable();

            if (!string.IsNullOrEmpty(status))
            {
                bool hieuLuc = status == "1";
                query = query.Where(u => u.HieuLuc == hieuLuc);
            }


            if (!string.IsNullOrEmpty(search))
            {
                query = query.Where(u =>
                    u.HoTen.Contains(search) ||
                    u.Email.Contains(search) ||
                    u.DienThoai.Contains(search)
                );
            }

            var customers = query
                .OrderBy(u => u.MaKh)
                .Select(u => new AccountVM
                {
                    MaKh = u.MaKh,
                    HoTen = u.HoTen,
                    Email = u.Email,
                    SoDienThoai = u.DienThoai,
                    DiaChi = u.DiaChi,
                    CapDo = "Vàng",
                    HieuLuc = u.HieuLuc,
                    Hinh = u.Hinh
                })
                .ToList();

            return View(customers); // Truyền trực tiếp danh sách vào view
        }

        public IActionResult Details(string id)
        {
            var kh = db.KhachHangs.FirstOrDefault(x => x.MaKh == id);

            if (kh == null)
                return NotFound();

            var vm = new AccountVM
            {
                MaKh = kh.MaKh,
                HoTen = kh.HoTen,
                Email = kh.Email,
                SoDienThoai = kh.DienThoai,
                DiaChi = kh.DiaChi,

                NgaySinh = kh.NgaySinh,
                GioiTinh = kh.GioiTinh,

                CapDo = "Vàng",
                HieuLuc = kh.HieuLuc,
                Hinh = kh.Hinh
            };

            return View(vm);
        }

        public IActionResult Create()
        {
            return View();
        }

        [HttpPost]
        public IActionResult Create(object model)
        {
            return RedirectToAction("Index");
        }

        public IActionResult Edit(string id)
        {
            var kh = db.KhachHangs.FirstOrDefault(x => x.MaKh == id);

            if (kh == null)
                return NotFound();

            var vm = new AccountVM
            {
                MaKh = kh.MaKh,
                HoTen = kh.HoTen,
                Email = kh.Email,
                SoDienThoai = kh.DienThoai,
                DiaChi = kh.DiaChi,

                NgaySinh = kh.NgaySinh,
                GioiTinh = kh.GioiTinh,

                CapDo = "Vàng",
                HieuLuc = kh.HieuLuc,
                Hinh = kh.Hinh
            };

            return View(vm);
        }

        [HttpPost]
        public async Task<IActionResult> Edit(AccountVM model, IFormFile? HinhFile)
        {
            if (!ModelState.IsValid)
            {
                return View(model);
            }

            var kh = await db.KhachHangs.FindAsync(model.MaKh);
            if (kh == null)
                return NotFound();

            if (HinhFile != null && HinhFile.Length > 0)
            {
                string folder = Path.Combine(_env.WebRootPath, "Client/Hinh/KhachHang");

                // Xóa ảnh cũ
                if (!string.IsNullOrEmpty(kh.Hinh))
                {
                    string oldPath = Path.Combine(folder, kh.Hinh);
                    if (System.IO.File.Exists(oldPath))
                        System.IO.File.Delete(oldPath);
                }

                // Lưu ảnh mới
                string fileName = $"{Guid.NewGuid()}{Path.GetExtension(HinhFile.FileName)}";
                string filePath = Path.Combine(folder, fileName);

                using (var stream = new FileStream(filePath, FileMode.Create))
                {
                    await HinhFile.CopyToAsync(stream);
                }

                kh.Hinh = fileName; // Cập nhật DB
            }

            kh.HoTen = model.HoTen;
            kh.DienThoai = model.SoDienThoai;
            kh.DiaChi = model.DiaChi;
            kh.NgaySinh = (DateTime)model.NgaySinh;
            kh.GioiTinh = model.GioiTinh;
            kh.HieuLuc = model.HieuLuc;

            await db.SaveChangesAsync();

            TempData["success"] = "Cập nhật thông tin khách hàng thành công!";
            return RedirectToAction("Details", new { id = model.MaKh });
        }

        [HttpPost]
        public IActionResult ToggleLock(string id)
        {
            var kh = db.KhachHangs.FirstOrDefault(x => x.MaKh == id);

            if (kh == null)
                return Json(new { success = false, message = "Không tìm thấy khách hàng" });

            // Đổi trạng thái
            kh.HieuLuc = !kh.HieuLuc;

            // Cập nhật trạng thái tài khoản (nếu bạn có bảng TaiKhoan thì cập nhật luôn)
            var tk = db.KhachHangs.FirstOrDefault(x => x.MaKh == id);
            if (tk != null)
                tk.HieuLuc = kh.HieuLuc ? true : false;

            db.SaveChanges();

            return Json(new
            {
                success = true,
                message = kh.HieuLuc ? "Đã mở khóa tài khoản" : "Đã khóa tài khoản",
                hieuLuc = kh.HieuLuc
            });
        }

        public IActionResult Delete(int id)
        {
            return View();
        }
    }
}