using AutoMapper;
using HShop.Models;
using HShop.Utils;
using HShop.ViewModels;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;

namespace HShop.Controllers
{
    public class KhachHangController : Controller
    {
        private readonly HshopContext db;
        private readonly IMapper _mapper;

        public KhachHangController(HshopContext context, IMapper mapper)
        {
            db = context;
            _mapper = mapper;
        }

        #region Register
        [HttpGet]
        public IActionResult Register()
        {
            return View();
        }

        [HttpPost]
        public IActionResult Register(RegisterVM model, IFormFile Hinh)
        {
            if (ModelState.IsValid)
            {
                var khachHang = _mapper.Map<KhachHang>(model);

                var checkExist = db.KhachHangs.FirstOrDefault(kh => kh.MaKh == khachHang.MaKh);
                if (checkExist != null)
                {
                    ViewData["Error"] = "Account existed";
                    return View(model);
                }
                
                khachHang.RandomKey = MySetting.GenerateRandomKey();
                khachHang.MatKhau = model.MatKhau.ToMd5Hash(khachHang.RandomKey);
                khachHang.HieuLuc = true; // will process when receive mail to active
                khachHang.VaiTro = 0;

                if (Hinh != null)
                {
                    khachHang.Hinh = MySetting.UploadImage(Hinh, "KhachHang");
                }

                db.Add(khachHang);
                db.SaveChanges();

                return RedirectToAction("Index", "HangHoa");
            }

            return View();
        }
        #endregion Register

        #region Login
        [HttpGet]
        public IActionResult Login(string? ReturnUrl)
        {
            ViewBag.ReturnUrl = ReturnUrl;
            return View();
        }

        [HttpPost]
        public async Task<IActionResult> Login(LoginVM model, string? ReturnUrl)
        {
            ViewBag.ReturnUrl = ReturnUrl;
            if (ModelState.IsValid)
            {
                var khachHang = db.KhachHangs.FirstOrDefault(kh => kh.MaKh == model.MaKh);

                if (khachHang == null)
                {
                    ModelState.AddModelError("Error", "Account not exist");
                }
                else if (!khachHang.HieuLuc)
                {
                    ModelState.AddModelError("Error", "Account expired. Please contact customer service staff!");
                }
                else
                {
                    var checkLogin = khachHang.MatKhau == model.MatKhau.ToMd5Hash(khachHang.RandomKey);

                    if (!checkLogin)
                    {
                        ModelState.AddModelError("Error", "Infomation login invalid!");
                    }
                    else
                    {
                        // Create an authentication cookie
                        // https://learn.microsoft.com/en-us/aspnet/core/security/authentication/cookie?view=aspnetcore-8.0
                        var claims = new List<Claim>
                        {
                            new Claim(ClaimTypes.Email, khachHang.Email),
                            new Claim(ClaimTypes.Name, khachHang.HoTen),
                            new Claim(MyConst.CLAM_CUSTOMER_ID, khachHang.MaKh),

                            // claim - role dynamic
                            new Claim(ClaimTypes.Role, "Customer")
                        };

                        var claimsIdentity = new ClaimsIdentity(
                            claims, CookieAuthenticationDefaults.AuthenticationScheme);
                        await HttpContext.SignInAsync(
                            CookieAuthenticationDefaults.AuthenticationScheme,
                            new ClaimsPrincipal(claimsIdentity));

                        if (Url.IsLocalUrl(ReturnUrl))
                        {
                            return Redirect(ReturnUrl);
                        }
                        else
                        {
                            return Redirect("/");
                        }
                    }
                }
            }
            return View();
        }
        #endregion Login

        [Authorize]
        public IActionResult Profile()
        {
            return View();
        }

        [Authorize]
        public async Task<IActionResult> Logout()
        {
            await HttpContext.SignOutAsync();
            return Redirect("/");
        }
    }
}
