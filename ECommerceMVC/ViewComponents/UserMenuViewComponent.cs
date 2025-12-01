using ECommerceMVC.Models.ViewModels;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;

namespace ECommerceMVC.ViewComponents
{
    public class UserMenuViewComponent : ViewComponent
    {
        public IViewComponentResult Invoke()
        {
            var isAuthenticated = HttpContext.User?.Identity?.IsAuthenticated ?? false;
            var username = HttpContext.User?.Identity?.Name;

            return View("Default", new UserMenuVM
            {
                IsAuthenticated = isAuthenticated,
                Username = username
            });
        }
    }
}
