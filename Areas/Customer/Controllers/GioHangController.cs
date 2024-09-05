using Microsoft.AspNetCore.Mvc;
using Demo.Data;
using Demo.Models;
using System.Security.Claims;
using Microsoft.EntityFrameworkCore;
using Microsoft.AspNetCore.Mvc.Rendering;

using Microsoft.AspNetCore.Authorization;
using Demo.Controllers;
// using PayPal.Api;


namespace Demo.Areas.Customer.Controllers
{
    [Area("Customer")]
    public class GioHangController : Controller
    {

        private readonly ApplicationDbContext _db;
        private string PaypalClientId { get; set; } = "";
        private string PaypalSecret { get; set; } = "";
        private string PaypalUrl { get; set; } = "";

        public GioHangController(ApplicationDbContext db, IConfiguration configuration)
        {

            _db = db;
            PaypalClientId = configuration["PaypalSettings:ClientId"];
            PaypalSecret = configuration["PaypalSettings:Secret"];
            PaypalUrl = configuration["PaypalSettings:Url"];
        }

        public IActionResult Index()
        {
            //lấy thông tin tài khoản
            var identity = (ClaimsIdentity)User.Identity;
            var claim = identity.FindFirst(ClaimTypes.NameIdentifier);
            //lẤY DANH SÁCH SẢN PHẨM TRONG GIỎ HÀNG CỦA USER
            //------------------------------------------------//
            //IEnumerable<GioHang> dsGioHang = _db.GioHang
            //	.Include("SanPham")
            //	.Where(gh => gh.ApplicationUserId == claim.Value)
            //	.ToList();
            //return View(dsGioHang);
            GioHangViewModel giohang = new GioHangViewModel()
            {
                DsGioHang = _db.GioHang.Include("SanPham").Where(gh => gh.ApplicationUserId == claim.Value).ToList(),
                HoaDon = new HoaDon()
            };
            foreach (var item in giohang.DsGioHang)
            {
                //TÍNH TIỀN THEO SỐ LƯỢNG SẢN PHẨM
                item.ProductPrice = item.Quantity * item.SanPham.Price;
                // Tính tổng số tiền trong giỏ hàng
                giohang.HoaDon.Total += item.ProductPrice;
            }
            return View(giohang);
        }

        public IActionResult Giam(int giohangId)
        {
            //Lấy Thông tin giỏ hàng tương ứng với giohangId
            var giohang = _db.GioHang.FirstOrDefault(gh => gh.Id == giohangId);
            //Giam số lượng sản phẩm giảm đi 1
            giohang.Quantity -= 1;
            //Nếu số lượng =0 thì xóa giỏ hàng
            if (giohang.Quantity == 0)
            {
                _db.GioHang.Remove(giohang);
            }
            //lưu lại CSDL
            _db.SaveChanges();
            //quay về trang giỏ hàng
            return RedirectToAction("Index");
        }

        public IActionResult Tang(int giohangId)
        {
            //Lấy Thông tin giỏ hàng tương ứng với giohangId
            var giohang = _db.GioHang.FirstOrDefault(gh => gh.Id == giohangId);
            //Giam số lượng sản phẩm tăng đi 1
            giohang.Quantity += 1;
            //lưu lại CSDL
            _db.SaveChanges();
            //quay về trang giỏ hàng
            return RedirectToAction("Index");
        }
        public IActionResult Xoa(int giohangId)
        {
            //lấy thông tin giỏ hàng tương ứng với giohangId
            var giohang = _db.GioHang.FirstOrDefault(gh => gh.Id == giohangId);
            //xóa giỏ hàng
            _db.GioHang.Remove(giohang);
            //Lưu lại csdl
            _db.SaveChanges();
            //trở vè
            return RedirectToAction("Index");
        }
        public IActionResult ThanhToan()
        {
            //lấy thông tin tài khoản
            var identity = (ClaimsIdentity)User.Identity;
            var claim = identity.FindFirst(ClaimTypes.NameIdentifier);
            //lẤY DANH SÁCH SẢN PHẨM TRONG GIỎ HÀNG CỦA USER
            //------------------------------------------------//
            //IEnumerable<GioHang> dsGioHang = _db.GioHang
            //	.Include("SanPham")
            //	.Where(gh => gh.ApplicationUserId == claim.Value)
            //	.ToList();
            //return View(dsGioHang);
            GioHangViewModel giohang = new GioHangViewModel()
            {
                DsGioHang = _db.GioHang.Include("SanPham").Where(gh => gh.ApplicationUserId == claim.Value).ToList(),
                HoaDon = new HoaDon()
            };
            //tìm thông tin tài khoản trong CSDL để hiện thị lên trang thanh toán
            giohang.HoaDon.ApplicationUser = _db.ApplicationUser.FirstOrDefault(User => User.Id == claim.Value);
            //gán thông tin tài khoản vào hóa đơn
            giohang.HoaDon.Name = giohang.HoaDon.ApplicationUser.Name;
            giohang.HoaDon.Address = giohang.HoaDon.ApplicationUser.Address;
            giohang.HoaDon.PhoneNumber = giohang.HoaDon.ApplicationUser.PhoneNumber;
            foreach (var item in giohang.DsGioHang)
            {
                //TÍNH TIỀN THEO SỐ LƯỢNG SẢN PHẨM
                item.ProductPrice = item.Quantity * item.SanPham.Price;
                // Tính tổng số tiền trong giỏ hàng
                giohang.HoaDon.Total += item.ProductPrice;
            }

            ViewBag.PaypalClientId = PaypalClientId;
            ViewBag.OrderId = giohang.HoaDon.Id;
            return View(giohang);
        }
        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult ThanhToan(GioHangViewModel giohang)
        {


            //lấy thông tin tài khoản
            var identity = (ClaimsIdentity)User.Identity;
            var claim = identity.FindFirst(ClaimTypes.NameIdentifier);
            //cập nhật thông tin danh sách giỏ hàng và hóa đơn
            giohang.DsGioHang = _db.GioHang.Include("SanPham").Where(gh => gh.ApplicationUserId == claim.Value).ToList();
            giohang.HoaDon.ApplicationUserId = claim.Value;
            giohang.HoaDon.OrderDate = DateTime.Now;
            giohang.HoaDon.OrderStatus = "Đang Xác Nhận";
            foreach (var item in giohang.DsGioHang)
            {
                //tính tiền sản phẩm theo số lượng
                item.ProductPrice = item.Quantity * item.SanPham.Price;
                //cộng dồn số tiền trong giỏ hàng
                giohang.HoaDon.Total += item.ProductPrice;
            }
            _db.HoaDon.Add(giohang.HoaDon);
            _db.SaveChanges();
            //thêm thông tin chi tiết hóa đơn
            foreach (var item in giohang.DsGioHang)
            {
                ChiTietHoaDon chiTietHoaDon = new ChiTietHoaDon()
                {
                    SanPhamId = item.SanPhamId,
                    HoaDonId = giohang.HoaDon.Id,
                    ProductPrice = item.ProductPrice,
                    Quantity = item.Quantity
                };
                _db.ChiTietHoaDon.Add(chiTietHoaDon);
                _db.SaveChanges();
            }
            // xóa thông tin trong giỏ hàng
            _db.GioHang.RemoveRange(giohang.DsGioHang);
            _db.SaveChanges();
            return RedirectToAction("Index", "Home");
        }

        [HttpPost]
        public JsonResult PaypalCheckout(GioHangViewModel giohang)
        {


            //lấy thông tin tài khoản
            var identity = (ClaimsIdentity)User.Identity;
            var claim = identity.FindFirst(ClaimTypes.NameIdentifier);
            //cập nhật thông tin danh sách giỏ hàng và hóa đơn
            giohang.DsGioHang = _db.GioHang.Include("SanPham").Where(gh => gh.ApplicationUserId == claim.Value).ToList();
            giohang.HoaDon.ApplicationUserId = claim.Value;
            giohang.HoaDon.OrderDate = DateTime.Now;
            giohang.HoaDon.OrderStatus = "Đang Xác Nhận";
            foreach (var item in giohang.DsGioHang)
            {
                //tính tiền sản phẩm theo số lượng
                item.ProductPrice = item.Quantity * item.SanPham.Price;
                //cộng dồn số tiền trong giỏ hàng
                giohang.HoaDon.Total += item.ProductPrice;
            }
            _db.HoaDon.Add(giohang.HoaDon);
            _db.SaveChanges();
            //thêm thông tin chi tiết hóa đơn
            foreach (var item in giohang.DsGioHang)
            {
                ChiTietHoaDon chiTietHoaDon = new ChiTietHoaDon()
                {
                    SanPhamId = item.SanPhamId,
                    HoaDonId = giohang.HoaDon.Id,
                    ProductPrice = item.ProductPrice,
                    Quantity = item.Quantity
                };
                _db.ChiTietHoaDon.Add(chiTietHoaDon);
                _db.SaveChanges();
            }
            // xóa thông tin trong giỏ hàng
            _db.GioHang.RemoveRange(giohang.DsGioHang);
            _db.SaveChanges();
            return new JsonResult(new
            {
                orderId = giohang.HoaDon.Id
            });
        }

        public IActionResult Checkout()
        {
            return View();
        }

        public IActionResult SuccessView()
        {
            return View();
        }
    }
}


