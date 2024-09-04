using Demo.Data;
using Demo.Models;
using Microsoft.AspNetCore.Mvc;

namespace Demo.Controllers
{
    public class NhaCungCapController : Controller
    {
        private readonly ApplicationDbContext _dbnhacungcap1;
        public NhaCungCapController(ApplicationDbContext nhacungcap12)
        {
            _dbnhacungcap1 = nhacungcap12;
        }
        public IActionResult Index()
        {
            var nhacungcap = _dbnhacungcap1.NhaCungCaps.ToList();
            ViewBag.nhaCungCaps = nhacungcap;
            return View();
        }
        


        [HttpGet]
        public IActionResult Create() // Hiện Thị tạo trang mới
        {
            return View();
        }

        [HttpPost] // Gửi dữ liệu lên controller để xử lý
        public IActionResult Create(NhaCungCap nha1) // cập nhật dữ liệu trong CSDL
        {
            // thêm thông tin vào bảng TheLoai trong CSDL
            _dbnhacungcap1.NhaCungCaps.Add(nha1);
            // Lưu lại
            _dbnhacungcap1.SaveChanges();
            return RedirectToAction("Index");// Trả về trang Index
        }
        


        [HttpGet] // Gửi dữ liệu và hiển thị
        public IActionResult Edit(int id)
        {
            if (id == 0)
            {                                                              /////// READ
                return NotFound();
            }
            // Tìm dòng dữ liệu và chỉnh sửa
            var nha1 = _dbnhacungcap1.NhaCungCaps.Find(id);
            return View(nha1);
        }
        [HttpPost]
        public IActionResult Edit(NhaCungCap nha1)
        {                                                                  /////////// UPDATE
            if (ModelState.IsValid)
            {
                _dbnhacungcap1.NhaCungCaps.Update(nha1);
                _dbnhacungcap1.SaveChanges();
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
            var nha1 = _dbnhacungcap1.NhaCungCaps.Find(id);
            return View(nha1);
        }

        [HttpPost]
        public ActionResult DeleteConfirm(int id) // đặt tên tránh trùng delete
        {
            var nha1 = _dbnhacungcap1.NhaCungCaps.Find(id);
            if (nha1 == null)
            {
                return NotFound();
            }
            //theLoai.IsDeleted = true; // Backup du lieu
            _dbnhacungcap1.NhaCungCaps.Remove(nha1);
            _dbnhacungcap1.SaveChanges();
            return RedirectToAction("Index");
        }
    }
}
