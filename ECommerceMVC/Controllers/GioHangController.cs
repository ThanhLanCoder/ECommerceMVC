using Microsoft.AspNetCore.Mvc;
using ECommerceMVC.Models.Entities;
using ECommerceMVC.Models.ViewModels;
using ECommerceMVC.Helpers;
namespace ECommerceMVC.Controllers
{
    public class GioHangController : Controller
    {
        private readonly EcomerceContext db;

        public GioHangController(EcomerceContext context)
        {
            db = context;
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

        private void SaveCart(List<CartItemVM> cart)
        {
            HttpContext.Session.SetObject("Cart", cart);
        }


        public IActionResult AddToCart(int id, int quantity = 1)
        {
            var product = db.HangHoas.FirstOrDefault(p => p.MaHh == id);
            if (product == null) return NotFound();

            var cart = GetCart();
            var item = cart.FirstOrDefault(c => c.MaHH == id);

            if (item == null)
            {
                cart.Add(new CartItemVM
                {
                    MaHH = id,
                    TenHH = product.TenHh,
                    Hinh = product.Hinh ?? "",
                    DonGia = product.DonGia ?? 0,
                    SoLuong = quantity,
                    SoLuongTon = 10
                });
            }
            else
            {
                item.SoLuong += quantity;
            }

            SaveCart(cart);

            return RedirectToAction("Index");
        }

        [HttpPost]
        public IActionResult UpdateQuantity(int id, int quantity)
        {
            var cart = GetCart();
            var item = cart.FirstOrDefault(c => c.MaHH == id);
            if (item == null)
                return Json(new { success = false });

            bool exceeded = false;
            string? message = null;

            int originalQty = item.SoLuong;
            int finalQty = quantity;

            if (finalQty < 1)
            {
                finalQty = 1;
                message = "Số lượng tối thiểu là 1.";
            }

            if (finalQty > item.SoLuongTon)
            {
                exceeded = true;
                finalQty = originalQty; // không cho tăng quá stock
                message = $"Trong kho chỉ còn {item.SoLuongTon} sản phẩm.";
            }

            item.SoLuong = finalQty;
            SaveCart(cart);

            return Json(new
            {
                success = true,
                finalQuantity = finalQty,
                thanhTien = item.ThanhTien,
                total = cart.Sum(x => x.ThanhTien),
                message,
                exceeded,
                stock = item.SoLuongTon
            });
        }

        [HttpPost]
        public IActionResult Remove(int id)
        {
            var cart = GetCart();
            var item = cart.FirstOrDefault(c => c.MaHH == id);
            if (item != null) { cart.Remove(item); SaveCart(cart); }

            return Json(new
            {
                success = true,
                total = cart.Sum(x => x.ThanhTien)
            });
        }

        public IActionResult Index()
        {
            var cart = GetCart();
            return View(cart);
        }
    }
}
