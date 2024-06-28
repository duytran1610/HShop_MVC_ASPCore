using AutoMapper;
using HShop.Models;
using HShop.Utils;
using HShop.ViewModels;
using Microsoft.AspNetCore.Mvc;

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
    }
}
