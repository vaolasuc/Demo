
using Demo.Data;
using Demo.Models;
using Microsoft.AspNetCore.Mvc;


namespace Demo.ViewComponents
{
    public class TheLoaiViewComponent : ViewComponent
    {
        private readonly ApplicationDbContext _db;
        public TheLoaiViewComponent (ApplicationDbContext db)
        {
            _db = db;
        }
        public IViewComponentResult Invoke()
        {
            IEnumerable<TheLoai> theloai = _db.Tentheloai.ToList();
            return View(theloai);
        }
    }
}
