using HShop.Models;
using HShop.Utils;
using HShop.ViewModels;
using Microsoft.AspNetCore.Mvc;

namespace HShop.ViewComponents
{
    public class CartViewComponent : ViewComponent
    {
        private readonly HshopContext db;

        public CartViewComponent(HshopContext context)
        {
            db = context;
        }

        public IViewComponentResult Invoke()
        {
            var cart = HttpContext.Session.Get<List<CartItemVM>>(MyConst.CART_KEY) ?? new List<CartItemVM>();
            var data = new CartModelVM
            {
                Quantity = cart.Sum(p => p.SoLuong),
                Total = cart.Sum(p => p.ThanhTien)
            };

            return View("CartPanel", data);  // CartPanel.cshtml
        }
    }
}
