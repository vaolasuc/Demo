using Demo.Data;
using Demo.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System.Diagnostics;
using System.Security.Claims;
namespace Demo.ViewComponents

{
    public class HintsViewComponent : ViewComponent
    {
        private readonly ApplicationDbContext _db;
        public HintsViewComponent(ApplicationDbContext db)
        {
            _db = db;
        }
        public IViewComponentResult Invoke()
        {
            IEnumerable<SanPham> sanPhams = _db.SanPham.Include(sp => sp.TheLoai).ToList();
            return View(sanPhams);
        }
    }
}
