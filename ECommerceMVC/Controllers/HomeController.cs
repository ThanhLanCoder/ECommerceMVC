using ECommerceMVC.Models;
using ECommerceMVC.Models.Entities;
using ECommerceMVC.Models.ViewModels;
using Microsoft.AspNetCore.Mvc;
using System.Diagnostics;

namespace ECommerceMVC.Controllers
{
    public class HomeController : Controller
    {
        private readonly EcomerceContext db;

        public HomeController(EcomerceContext context)
        {
            db = context;
        }

        public IActionResult Index()
        {
            return RedirectToAction("Index", "HangHoa");
        }

        public IActionResult Privacy()
        {
            return View();
        }

        [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
        public IActionResult Error()
        {
            return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
        }
    }
}
