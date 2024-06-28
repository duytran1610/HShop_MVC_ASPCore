using HShop.Models;
using HShop.ViewModels;
using Microsoft.AspNetCore.Mvc;
using HShop.Utils;
using System.Collections.Generic;

namespace HShop.Controllers
{
    public class CartController : Controller
    {
        private readonly HshopContext db;

        public List<CartItemVM> Cart
        {
            get
            {
                return HttpContext.Session.Get<List<CartItemVM>>(MyConst.CART_KEY) ?? new List<CartItemVM>();
            }
        }

        public CartController(HshopContext context)
        {
            db = context;
        }

        public IActionResult Index()
        {
            return View(Cart);
        }

        public IActionResult Add(int id, int quantity = 1)
        {
            var gioHang = Cart;
            var item = gioHang.SingleOrDefault(p => p.MaHh == id);

            if (item == null)
            {
                var hangHoa = db.HangHoas.SingleOrDefault(p => p.MaHh == id);

                if (hangHoa == null)
                {
                    TempData["Message"] = $"Product id {id} not found!";
                    return Redirect("/404");
                }

                item = new CartItemVM
                {
                    MaHh = hangHoa.MaHh,
                    TenHh = hangHoa.TenHh,
                    DonGia = hangHoa.DonGia ?? 0,
                    Hinh = hangHoa.Hinh ?? string.Empty,
                    SoLuong = quantity
                };

                gioHang.Add(item);
            }
            else
            {
                item.SoLuong += quantity;
            }

            HttpContext.Session.Set<List<CartItemVM>>(MyConst.CART_KEY, gioHang);

            return RedirectToAction("Index");
        }

        public IActionResult Remove(int id)
        {
            var gioHang = Cart;
            var item = gioHang.SingleOrDefault(p => p.MaHh == id);

            if (item != null)
            {
                gioHang.Remove(item);
                HttpContext.Session.Set<List<CartItemVM>>(MyConst.CART_KEY, gioHang);
            }

            return RedirectToAction("Index");
        }
    }
}
