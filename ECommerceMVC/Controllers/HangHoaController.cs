using ECommerceMVC.Helper;
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
        public async Task<IActionResult> IndexAsync(int? loai, int page = 1, string? search = null)
        {
            int pageSize = 9;

            var query = db.HangHoas.AsQueryable();

            if (loai.HasValue)
                query = query.Where(p => p.MaLoai == loai.Value);

            if (!string.IsNullOrEmpty(search))
            {
                query = query.Where(h =>
                    h.TenHh.Contains(search) ||
                    h.MoTaDonVi.Contains(search));
            }

            var projectedQuery = query
                .OrderBy(p => p.TenHh)
                .Select(p => new HangHoaVM
                {
                    MaHH = p.MaHh,
                    TenHH = p.TenHh,
                    DonGia = p.DonGia ?? 0,
                    Hinh = p.Hinh ?? "",
                    MoTaNgan = p.MoTaDonVi ?? "",
                    TenLoai = p.MaLoaiNavigation.TenLoai
                });

            var pagedData = await projectedQuery.ToPagedListAsync(page, pageSize);

            ViewBag.Loai = loai;
            ViewBag.Search = search;

            return View(pagedData);
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
