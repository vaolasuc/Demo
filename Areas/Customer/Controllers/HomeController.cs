using Demo.Data;
using Demo.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System.Diagnostics;
using System.Security.Claims;

namespace Demo.Areas.Customer.Controllers
{
    [Area("Customer")]
    public class HomeController : Controller
    {
        private readonly ILogger<HomeController> _logger;
        private readonly ApplicationDbContext _db;
        public HomeController(ILogger<HomeController> logger, ApplicationDbContext db)
        {
            _logger = logger;
            _db = db;
        }
        public IActionResult Index()
        {
            IEnumerable<SanPham> sanPhams = _db.SanPham.Include(sp =>sp.TheLoai).ToList();
            return View(sanPhams);
        }



        //public IActionResult Index()
        //{
        //    ViewBag.Nghia = "nghiaburh";
        //    ViewBag.Nghia1 = "1412";
        //    ViewData["Nam"] = "Nam 2003";
        //    var Nghia = new Theloaiviewmodel
        //    {
        //        Id = 1,
        //        Name = "Test",
        //    };
        //    return View(Nghia);//dùng viewmodel phải chú ý phần này
        //}

        public IActionResult Privacy()
        {
            return View();
        }

        [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
        public IActionResult Error()
        {
            return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
        }

        //public IActionResult Detail(int id, string ten)
        //{
        //    return Content("id : " + id + "; ten: " + ten) ;
        //}

        public IActionResult Detail(int id, string ten)
        {
            return Content(string.Format("id :{0}; ten:{1} ", id, ten));
        }

        //public IActionResult Show(string categories)
        //{
        //    return Content(string.Format("Category List: {0}; {1}; {2}; {3}", categories, categories, categories, categories));
        //}

        public IActionResult Show(List<string> categories)
        {
            string content = "Category List: ";
            foreach (var category in categories)
            {
                // Nối chuỗi: SV tự tạo code
                content += category + "; ";
            }
            return Content(content);
        }
        public IActionResult TrangChu(int id) 
        {

            IEnumerable<SanPham> sanPhams = _db.SanPham.Include(sp =>sp.TheLoai).ToList();
            return View(sanPhams);

        }
        [HttpGet]
        public ActionResult Details(int sanphamId)
        {
            //Tạo giỏ hàng ở trang Details để xử lý chức năng thêm vào giỏ sau này
            GioHang gioHang = new GioHang()
            {
                SanPhamId = sanphamId,
                SanPham = _db.SanPham.Include("TheLoai").FirstOrDefault(sp => sp.Id == sanphamId),
                Quantity = 1
            };//mặc định trở về 1
            
            return View(gioHang);
        }
        [HttpPost]
        [Authorize]//YÊU CẦU ĐĂNG NHẬP
        public IActionResult Details(GioHang gioHang)
        {
            //LẤY THÔNG TIN TÀI KHOẢN
            var identity = (ClaimsIdentity)User.Identity;
            var claim = identity.FindFirst(ClaimTypes.NameIdentifier);
            gioHang.ApplicationUserId = claim.Value;
            //Kiểm tra sản phẩm đã có trong giỏ hàng chưa
            var giohangdb = _db.GioHang.FirstOrDefault(sp => sp.SanPhamId == gioHang.SanPhamId
            && sp.ApplicationUserId == gioHang.ApplicationUserId);
            if (giohangdb == null)//nếu không có sản phẩm trong giỏ hàng
            {
                _db.GioHang.Add(gioHang);//thêm sản phẩm vào giỏ hàng
            }
            else // nếu  có sản phẩm trong giỏ hàng
            {
                giohangdb.Quantity += gioHang.Quantity;//update số lượng
            }
            // THÊM SẢN PHẨM VÀO GIỎ HÀNG
            
			_db.SaveChanges();
            return RedirectToAction("Index", "GioHang");
        }

        public IActionResult ConTact() { return View(); }
        public IActionResult About() { return View(); }
        public IActionResult Ser() { return View(); }
        public IActionResult Blog() { return View(); }
    }
}