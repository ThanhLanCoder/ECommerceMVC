using ECommerceMVC.Models;
using ECommerceMVC.Models.Entities;
using ECommerceMVC.Models.ViewModels;
using Microsoft.AspNetCore.Mvc;


namespace ECommerceMVC.Controllers
{
    public class HangHoaController : Controller
    {
        private readonly EcomerceContext db;

        public HangHoaController(EcomerceContext context) {
            db = context;
        }
        public IActionResult Index(int? loai, int page = 1, string? search = null)
        {
            int pageSize = 9; 

            var query = db.HangHoas.AsQueryable();

            // lọc theo danh mục
            if (loai.HasValue)
                query = query.Where(p => p.MaLoai == loai.Value);

            // lọc theo từ khóa tìm kiếm
            if (!string.IsNullOrEmpty(search))
            {
                query = query.Where(h =>
                    h.TenHh.Contains(search) ||
                    h.MoTaDonVi.Contains(search));
            }

            // tổng số sản phẩm sau khi lọc
            int totalItems = query.Count();

            // lấy sản phẩm phân trang
            var hangHoas = query
                .OrderBy(p => p.TenHh)
                .Skip((page - 1) * pageSize)
                .Take(pageSize)
                .Select(p => new HangHoaVM
                {
                    MaHH = p.MaHh,
                    TenHH = p.TenHh,
                    DonGia = p.DonGia ?? 0,
                    Hinh = p.Hinh ?? "",
                    MoTaNgan = p.MoTaDonVi ?? "",
                    TenLoai = p.MaLoaiNavigation.TenLoai
                }).ToList();

            int totalPages = (int)Math.Ceiling(totalItems / (double)pageSize);

            var model = new PhanTrangHangHoaVM
            {
                HangHoas = hangHoas,
                CurrentPage = page,
                TotalPages = totalPages,
                MaLoai = loai,
                Search = search
            };

            return View(model);
        }
        public IActionResult Detail(int id)
        {
            var hangHoa = db.HangHoas
                .Where(p => p.MaHh == id)
                .Select(p => new ChiTietSanPhamVM
                {
                    MaHH = p.MaHh,
                    TenHH = p.TenHh,
                    DonGia = p.DonGia ?? 0,
                    Hinh = p.Hinh ?? "",
                    MoTaNgan = p.MoTaDonVi ?? "",
                    TenLoai = p.MaLoaiNavigation.TenLoai,
                    MoTaChiTiet = p.MoTa ?? "",
                    DiemDanhGia = 5,
                    SoLuongTon = 10,

                })
                .FirstOrDefault();
            if (hangHoa == null)
            {
                return NotFound();
            }
            return View("ProductDetail", hangHoa);
        }
    }
}
