using HShop.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using HShop.ViewModels;

namespace HShop.ViewComponents
{
    public class MenuLoaiViewComponent : ViewComponent
    {
        private readonly HshopContext db;

        public MenuLoaiViewComponent(HshopContext context)
        {
            db = context;
        }

        public async Task<IViewComponentResult> InvokeAsync()
        {
            var data = await db.Loais.Select(loai => new MenuLoaiVM
            {
                MaLoai = loai.MaLoai,
                TenLoai = loai.TenLoai, 
                SoLuong = loai.HangHoas.Count
            }).OrderBy(p => p.TenLoai).ToListAsync();

            return View(data);  // Default.cshtml
        }
    }
}
