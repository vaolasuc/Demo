using Demo.Data;
using Demo.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System.Diagnostics;
using System.Security.Claims;

namespace Demo.ViewComponents
{
    public class CongDonViewComponent : ViewComponent
    {
        private readonly ApplicationDbContext _db;
        public CongDonViewComponent(ApplicationDbContext db)
        {
            _db = db;
        }
        public IViewComponentResult Invoke()
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
    }
}
