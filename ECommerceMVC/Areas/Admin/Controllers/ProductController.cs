using ECommerceMVC.Areas.Admin.Models.ViewModels;
using ECommerceMVC.Helper;
using ECommerceMVC.Models.Common;
using ECommerceMVC.Models.Entities;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System.Linq;

namespace ECommerceMVC.Areas.Admin.Controllers
{
    [Area("Admin")]
    public class ProductController : Controller
    {
        private readonly EcomerceContext db;

        public ProductController(EcomerceContext context)
        {
            db = context;
        }

        public IActionResult Index(int page = 1, string? search = null, int? category = null)
        {
            int pageSize = 10;

            // Query cơ bản
            var query = db.HangHoas.AsQueryable();

            // Lọc theo danh mục nếu có
            if (category.HasValue)
                query = query.Where(h => h.MaLoai == category.Value);

            // Lọc theo từ khóa tìm kiếm nếu có
            if (!string.IsNullOrEmpty(search))
                query = query.Where(h => h.TenHh.Contains(search) || h.MoTaDonVi.Contains(search));

            // Project sang ViewModel
            var projectedQuery = query
                .OrderBy(h => h.MaHh)
                .Select(h => new ProductItemVM
                {
                    MaHH = h.MaHh,
                    TenHH = h.TenHh,
                    DonGia = h.DonGia ?? 0,
                    Hinh = h.Hinh ?? "",
                    MoTaNgan = h.MoTaDonVi ?? "",
                    MoTaChiTiet = h.MoTa ?? "",
                    SoLuongTon = 10,
                    TenLoai = h.MaLoaiNavigation.TenLoai
                });

            // Dùng PagingExtensions nếu có
            var pagedData = projectedQuery.ToPagedListAsync(page, pageSize).Result;

            // Truyền dữ liệu search & category để giữ filter khi phân trang
            ViewBag.Search = search;
            ViewBag.Category = category;

            // Trả về View với PagedResult<ProductItemVM>
            return View(pagedData);
        }

        public IActionResult Details(int id)
        {
            var product = db.HangHoas
                .Where(h => h.MaHh == id)
                .Select(h => new ProductItemVM
                {
                    MaHH = h.MaHh,
                    TenHH = h.TenHh,
                    DonGia = h.DonGia ?? 0,
                    Hinh = h.Hinh ?? "",
                    MoTaNgan = h.MoTaDonVi ?? "",
                    MoTaChiTiet = h.MoTa ?? "",
                    SoLuongTon = 10,
                    TenLoai = h.MaLoaiNavigation.TenLoai
                })
                .FirstOrDefault();

            if (product == null)
                return NotFound();

            return View(product);
        }

        public IActionResult Create()
        {
            ViewBag.Categories = db.Loais.Select(l => l.TenLoai).ToList();
            return View();
        }

        [HttpPost]
        public IActionResult Create(ProductItemVM model, IFormFile? HinhFile)
        {
            if (ModelState.IsValid)
            {
                // Xử lý upload hình ảnh
                string hinhFileName = "";
                if (HinhFile != null)
                {
                    hinhFileName = MyUntil.UploadImage(HinhFile, "HangHoa");
                }

                var entity = new HangHoa
                {
                    TenHh = model.TenHH,
                    DonGia = model.DonGia,
                    Hinh = hinhFileName, // Lưu tên file
                    MoTaDonVi = model.MoTaNgan,
                    MoTa = model.MoTaChiTiet,
                    MaLoai = db.Loais.FirstOrDefault(l => l.TenLoai == model.TenLoai)?.MaLoai ?? 0
                };
                db.HangHoas.Add(entity);
                db.SaveChanges();
                return RedirectToAction("Index");
            }

            // Nếu có lỗi, load lại danh sách categories
            ViewBag.Categories = db.Loais.Select(l => l.TenLoai).ToList();
            return View(model);
        }

        public IActionResult Edit(int id)
        {
            // SỬA: Dùng Include để load navigation property
            var product = db.HangHoas
                .Include(h => h.MaLoaiNavigation)
                .FirstOrDefault(h => h.MaHh == id);

            if (product == null) return NotFound();

            var model = new ProductItemVM
            {
                MaHH = product.MaHh,
                TenHH = product.TenHh,
                DonGia = product.DonGia ?? 0,
                Hinh = product.Hinh ?? "",
                MoTaNgan = product.MoTaDonVi ?? "",
                MoTaChiTiet = product.MoTa ?? "",
                TenLoai = product.MaLoaiNavigation?.TenLoai ?? "" // Thêm null check
            };

            // Load danh sách categories
            ViewBag.Categories = db.Loais.Select(l => l.TenLoai).ToList();

            return View(model);
        }

        [HttpPost]
        public IActionResult Edit(int id, ProductItemVM model, IFormFile? HinhFile)
        {
            if (ModelState.IsValid)
            {
                var product = db.HangHoas.Find(id);
                if (product == null) return NotFound();

                // Xử lý upload hình ảnh mới (nếu có)
                if (HinhFile != null)
                {
                    string hinhFileName = MyUntil.UploadImage(HinhFile, "HangHoa");
                    if (!string.IsNullOrEmpty(hinhFileName))
                    {
                        product.Hinh = hinhFileName;
                    }
                }

                product.TenHh = model.TenHH;
                product.DonGia = model.DonGia;
                product.MoTaDonVi = model.MoTaNgan;
                product.MoTa = model.MoTaChiTiet;
                product.MaLoai = db.Loais.FirstOrDefault(l => l.TenLoai == model.TenLoai)?.MaLoai ?? product.MaLoai;

                db.SaveChanges();
                return RedirectToAction("Index");
            }

            // Nếu có lỗi, load lại danh sách categories
            ViewBag.Categories = db.Loais.Select(l => l.TenLoai).ToList();
            return View(model);
        }


        public IActionResult Delete(int id)
        {
            var product = db.HangHoas
                .Include(h => h.MaLoaiNavigation)
                .FirstOrDefault(h => h.MaHh == id);

            if (product == null) return NotFound();

            // Kiểm tra xem sản phẩm có trong hóa đơn không
            var hasOrders = db.ChiTietHds.Any(ct => ct.MaHh == id);
            ViewBag.HasOrders = hasOrders;

            return View(product);
        }

        [HttpPost, ActionName("Delete")]
        public IActionResult DeleteConfirmed(int id)
        {
            var product = db.HangHoas.Find(id);
            if (product == null) return NotFound();

            // Kiểm tra xem sản phẩm có trong hóa đơn không
            var hasOrders = db.ChiTietHds.Any(ct => ct.MaHh == id);

            if (hasOrders)
            {
                TempData["ErrorMessage"] = "Không thể xóa sản phẩm này vì đã có trong hóa đơn!";
                return RedirectToAction("Delete", new { id = id });
            }

            try
            {
                db.HangHoas.Remove(product);
                db.SaveChanges();
                TempData["SuccessMessage"] = "Xóa sản phẩm thành công!";
                return RedirectToAction("Index");
            }
            catch (Exception ex)
            {
                TempData["ErrorMessage"] = "Có lỗi xảy ra khi xóa sản phẩm: " + ex.Message;
                return RedirectToAction("Delete", new { id = id });
            }
        }
    }
}
