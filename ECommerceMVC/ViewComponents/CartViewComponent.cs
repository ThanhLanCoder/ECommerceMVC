using Microsoft.AspNetCore.Mvc;
using ECommerceMVC.Models.ViewModels;
using ECommerceMVC.Helpers;

namespace ECommerceMVC.ViewComponents
{
    public class CartViewComponent : ViewComponent
    {
        public IViewComponentResult Invoke()
        {
            var cart = HttpContext.Session.GetObject<List<CartItemVM>>("Cart")
                       ?? new List<CartItemVM>();

            int totalItems = cart.Sum(x => x.SoLuong);

            return View(totalItems);
        }
    }
}