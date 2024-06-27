using HShop.Models;
using HShop.ViewModels;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace HShop.Controllers
{
    public class HangHoaController : Controller
    {
        private readonly HshopContext db;

        public HangHoaController(HshopContext context)
        {
            db = context;
        }

        public IActionResult Index(int? loai)
        {
            var hangHoas = db.HangHoas.AsQueryable();

            if (loai.HasValue)
            {
                hangHoas = hangHoas.Where(p => p.MaLoai == loai.Value); 
            }

            var result = hangHoas.Select(p => new HangHoaVM
            {
                MaHh = p.MaHh,
                TenHh = p.TenHh,
                DonGia = p.DonGia ?? 0,
                Hinh = p.Hinh ?? "",
                MotaNgan = p.MoTaDonVi ?? "",
                TenLoai = p.MaLoaiNavigation.TenLoai
            });

            return View(result);
        }

        public IActionResult Search(string? query)
        {
            var hangHoas = db.HangHoas.AsQueryable();

            if (!string.IsNullOrEmpty(query))
            {
                hangHoas = hangHoas.Where(p => p.TenHh.Contains(query));
            }

            var result = hangHoas.Select(p => new HangHoaVM
            {
                MaHh = p.MaHh,
                TenHh = p.TenHh,
                DonGia = p.DonGia ?? 0,
                Hinh = p.Hinh ?? "",
                MotaNgan = p.MoTaDonVi ?? "",
                TenLoai = p.MaLoaiNavigation.TenLoai
            });

            return View(result);
        }

        public IActionResult Detail(int id)
        {
            var data = db.HangHoas.Include(p => p.MaLoaiNavigation)
                .SingleOrDefault(p => p.MaHh == id);

            if (data == null)
            {
                TempData["Message"] = $"Product id {id} not found!";
                return Redirect("/404");
            }

            var result = new DetailHangHoaVM
            {
                MaHh = data.MaHh,
                TenHh = data.TenHh,
                DonGia = data.DonGia ?? 0,
                ChiTiet = data.MoTa ?? string.Empty,
                Hinh = data.Hinh ?? string.Empty,
                MotaNgan = data.MoTaDonVi ?? string.Empty,
                TenLoai = data.MaLoaiNavigation.TenLoai,
                SoLuongTon = 10, // tinh sau
                DiemDanhGia = 5 // tinh sau
            };

            return View(result);
        }
    }
}
