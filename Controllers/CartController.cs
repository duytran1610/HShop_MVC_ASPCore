using HShop.Models;
using HShop.ViewModels;
using Microsoft.AspNetCore.Mvc;
using HShop.Utils;
using System.Collections.Generic;
using Microsoft.AspNetCore.Authorization;
using HShop.Services;

namespace HShop.Controllers
{
    public class CartController : Controller
    {
        private readonly HshopContext db;
        private readonly PaypalClient _paypalClient;

        public List<CartItemVM> Cart
        {
            get
            {
                return HttpContext.Session.Get<List<CartItemVM>>(MyConst.CART_KEY) ?? new List<CartItemVM>();
            }
        }

        public CartController(HshopContext context, PaypalClient paypalClient)
        {
            db = context;
            _paypalClient = paypalClient;
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

        #region Checkout
        [Authorize]
        [HttpGet]
        public IActionResult Checkout()
        {
            if (Cart.Count == 0)
            {
                return Redirect("/");
            }

            ViewBag.PaypalClientId = _paypalClient.ClientId;

            return View(Cart);
        }

        [Authorize]
        [HttpPost]
        public IActionResult Checkout(CheckoutVM model)
        {
            if (ModelState.IsValid)
            {
                var customerId = HttpContext.User.Claims.FirstOrDefault(p => p.Type == MyConst.CLAM_CUSTOMER_ID)!.Value;
                var khachHang = new KhachHang();

                if (model.GiongKhachHang)
                {
                    khachHang = db.KhachHangs.SingleOrDefault(kh => kh.MaKh == customerId);
                }

                var hoaDon = new HoaDon
                {
                    MaKh = customerId,
                    HoTen = model.HoTen ?? khachHang?.HoTen,
                    DiaChi = model.DiaChi ?? khachHang.DiaChi,
                    DienThoai = model.DienThoai ?? khachHang?.DienThoai,
                    NgayDat = DateTime.Now,
                    CachThanhToan = "COD",
                    CachVanChuyen = "GRAB",
                    MaTrangThai = 0,
                    GhiChu = model.GhiChu
                };

                db.Database.BeginTransaction();
                try
                {
                    db.Database.CommitTransaction();
                    db.Add(hoaDon);
                    db.SaveChanges();

                    var cthds = new List<ChiTietHd>();

                    foreach(var item in Cart)
                    {
                        cthds.Add(new ChiTietHd
                        {
                            MaHd = hoaDon.MaHd,
                            SoLuong = item.SoLuong,
                            DonGia = item.DonGia,
                            MaHh = item.MaHh,
                            GiamGia = 0
                        });
                    }

                    db.AddRange(cthds);
                    db.SaveChanges();

                    // Đặt lại session lưu trữ giá trị giỏ hàng sau khi checkout
                    HttpContext.Session.Set<List<CartItemVM>>(MyConst.CART_KEY, new List<CartItemVM>());

                    return View("Success");
                }
                catch
                {
                    db.Database.RollbackTransaction();
                }
            }

            return View(Cart);
        }
        #endregion Checkout

        #region Paypal payment
        [Authorize]
        [HttpPost("/Cart/create-paypal-order")]
        public async Task<IActionResult> CreatePaypalOrder(CancellationToken cancellationToken)
        {
            // Information order gửi qua Paypal
            var tongTien = Cart.Sum(p => p.ThanhTien).ToString();
            var donViTienTe = "USD";
            var maDonHangThamChieu = "DH" + DateTime.Now.Ticks.ToString();

            try
            {
                var response = await _paypalClient.CreateOrder(tongTien, donViTienTe, maDonHangThamChieu);

                return Ok(response);
            }
            catch(Exception err)
            {
                var error = new { err.GetBaseException().Message };

                return BadRequest(error);
            }
        }

        [Authorize]
        [HttpPost("/Cart/capture-paypal-order")] 
        public async Task<IActionResult> CapturePaypalOrder(string orderID, CancellationToken cancellationToken)
        {
            try
            {
                var response = await _paypalClient.CaptureOrder(orderID);

                // Save data donHang
                // ....

                return Ok(response);
            }
            catch (Exception err)
            {
                var error = new { err.GetBaseException().Message };

                return BadRequest(error);
            }
        }

        [Authorize]
        public IActionResult PaymentSuccess()
        {
            return View("Success");
        }
        #endregion Paypal payment
    }
}
