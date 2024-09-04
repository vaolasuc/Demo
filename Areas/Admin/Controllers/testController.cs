using Demo.Data;
using Demo.Models;
using Microsoft.AspNetCore.Mvc;

namespace Demo.Areas.Admin.Controllers
{
    [Area("Admin")]
    public class testController : Controller
    {

        // -----------------------------------------------------------//

        private readonly ApplicationDbContext _db;
        public testController(ApplicationDbContext db)
        {
            _db = db;
        }
        public IActionResult Index()
        {
            var theloai = _db.Tentheloai.ToList();
            ViewBag.tentheloai = theloai;
            return View();
        }
        [HttpGet] // lấy dữ liệu và xuất ra trang mới
        public IActionResult Create() // Hiện Thị tạo trang mới
        {
            return View();
        }

        [HttpPost] // Gửi dữ liệu lên controller để xử lý
        public IActionResult Create(TheLoai theLoai) // cập nhật dữ liệu trong CSDL
        {
            if (ModelState.IsValid) // Load lai trang
            {
                // thêm thông tin vào bảng TheLoai trong CSDL
                _db.Tentheloai.Add(theLoai);
                // Lưu lại
                _db.SaveChanges();
                return RedirectToAction("Index");// Trả về trang Index
            }
            return View();
        }


        [HttpGet] // Gửi dữ liệu và hiển thị
        public IActionResult Edit(int id)
        {
            if (id == 0)
            {                                                              /////// READ
                return NotFound();
            }
            // Tìm dòng dữ liệu và chỉnh sửa
            var theLoai = _db.Tentheloai.Find(id);
            return View(theLoai);
        }

        [HttpPost]
        public IActionResult Edit(TheLoai theLoai)
        {                                                                  /////////// UPDATE
            if (ModelState.IsValid)
            {
                _db.Tentheloai.Update(theLoai);
                _db.SaveChanges();
                return RedirectToAction("Index");
            }
            return View();
        }


        [HttpGet]
        public ActionResult Delete(int id)
        {
            if (id == 0)
            {
                return NotFound();
            }
            var theLoai = _db.Tentheloai.Find(id);
            return View(theLoai);
        }

        [HttpPost]
        public ActionResult DeleteConfirm(int id) // đặt tên tránh trùng delete
        {
            var theLoai = _db.Tentheloai.Find(id);
            if (theLoai == null)
            {
                return NotFound();
            }
            //theLoai.IsDeleted = true; // Backup du lieu
            _db.Tentheloai.Remove(theLoai);
            _db.SaveChanges();
            return RedirectToAction("Index");
        }

        [HttpGet]
        public ActionResult Details(int id)
        {
            if (id == 0)
            {
                return NotFound();
            }
            var theLoai = _db.Tentheloai.Find(id);
            return View(theLoai);
        }

        [HttpPost]
        public ActionResult Details(TheLoai theLoai)
        {
            return View(theLoai);
        }


        public IActionResult Error()
        {
            return View();
        }
    }
}