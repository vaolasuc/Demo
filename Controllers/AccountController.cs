using Demo.Models;
using Microsoft.AspNetCore.Mvc;
using System.Collections.Specialized;
using System.Security.Cryptography;

namespace Demo.Controllers
{
    public class AccountController : Controller
    {
        public IActionResult Index()
        {
            return View();
        }
        // Trang đăng kí
        public IActionResult Signup(AccountViewModel account)
        {
            if (account.Username != null)
            {
                return Content(account.Username);

            }
            return View();
        }
        //public IActionResult BaiTap02() 
        //{ 
        //    var sanpham = new SanPhamViewModel();
        //    {
        //        sanpham = 
        //    }

        //}
    }
}