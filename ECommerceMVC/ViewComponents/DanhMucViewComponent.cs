using ECommerceMVC.Models.Entities;
using ECommerceMVC.Models.ViewModels;
using Microsoft.AspNetCore.Mvc;

namespace ECommerceMVC.ViewComponents
{
    public class DanhMucViewComponent : ViewComponent
    {
        private readonly EcomerceContext db;

        public DanhMucViewComponent(EcomerceContext context) => db = context;

        public IViewComponentResult Invoke()
        {
            var danhmucs = db.Loais.Select(l => new DanhMucVM
            {
                MaLoai = l.MaLoai,
                TenLoai = l.TenLoai,
                SoLuong = l.HangHoas.Count
            }).OrderBy(p => p.TenLoai);
            return View(danhmucs);
        }
    }
}
