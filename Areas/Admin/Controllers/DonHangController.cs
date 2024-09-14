using Demo.Data;
using Demo.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace Demo.Areas.Admin.Controllers
{
    [Authorize(Roles = "Admin")]
    [Area("Admin")]
    public class DonHangController : Controller
    {
        private readonly ApplicationDbContext _db;
        public DonHangController(ApplicationDbContext db)
        {
            _db = db;
        }
        public IActionResult Index()
        {
            // Check if user is authenticated
            if (!User.Identity.IsAuthenticated)
            {
                // Check role
                if (!User.IsInRole("Admin"))
                {

                    return RedirectToAction("Index", "Home", new { area = "Customer" });
                }

                return RedirectToAction("Index", "DonHang", new { area = "Admin" });

            }

            IEnumerable<HoaDon> hoadon = _db.HoaDon.Include("ApplicationUser").ToList();
            return View(hoadon);
        }
    }
}
