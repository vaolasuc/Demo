using Demo.Data;
using Demo.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;

namespace Demo.Areas.Admin.Controllers
{
	[Authorize(Roles = "Admin")]
	[Area("Admin")]
	public class SanPhamController : Controller
	{
		private readonly ApplicationDbContext _db;
		public SanPhamController(ApplicationDbContext db)
		{
			_db = db;
		}
		public IActionResult Index()
		{
			// Lấy thông tin trong bảng sản phẩm và bao gồm thêm thông tin  bảng TheLoai
			IEnumerable<SanPham> sanPhams = _db.SanPham.Include("TheLoai").ToList();
			return View(sanPhams);
		}
		[HttpGet]
		public IActionResult Upsert(int id)
		{
			SanPham sanPham = new SanPham();
			IEnumerable<SelectListItem> dstheloai = _db.Tentheloai.Select(
				item => new SelectListItem
				{
					Value = item.Id.ToString(),
					Text = item.Name,
				}
				);
			ViewBag.DSTheLoai = dstheloai;
			if (id == 0)  // create / Insert
			{
				return View(sanPham);
			}
			else // Edit / Update
			{
				sanPham = _db.SanPham.Include("TheLoai").FirstOrDefault(sp => sp.Id == id);
				return View(sanPham);
			}
		}

		[HttpPost]
        public IActionResult Upsert(SanPham sanPham)
		{
			if (ModelState.IsValid)
			{
				if(sanPham.Id == 0)
				{
					_db.SanPham.Add(sanPham);
				}
				else
				{
					_db.SanPham.Update(sanPham);
				}
				_db.SaveChanges();
				return RedirectToAction("Index");
			}
			return View();
		}

		[HttpPost]
		public IActionResult Delete(int id)
		{
			var sanPham = _db.SanPham.FirstOrDefault(sp => sp.Id == id);
			if(sanPham == null)
			{
				return NotFound();
			}
			_db.SanPham.Remove(sanPham);
			_db.SaveChanges();
			return Json(new { success = true });
		}

        //[HttpGet]
        //public async Task<IActionResult> Index(string Empsearch)
        //{
        //	ViewData["Getemployeedetails"] = Empsearch;
        //	var emqquery = from x in _db.SanPham select x;
        //	if(!String.IsNullOrEmpty(Empsearch) )
        //	{
        //		emqquery = emqquery.Where(x => x.TheLoai.Name.Contains(Empsearch) || x.Description.Equals(Empsearch));
        //	}
        //	return View(await emqquery.AsNoTracking().ToListAsync());
        //}
        

    }
}
