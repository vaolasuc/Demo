namespace Demo.Models
{
    public class GioHangViewModel
    {
        //LƯU TRỮ THÔNG TIN CÁC SẢN PHẨM TRONG GIỎ HÀNG
        public IEnumerable<GioHang> DsGioHang { get; set; }
        //LƯU TRỮ TỔNG SỐ TIỀN CỦA GIỎ HÀNG
        public HoaDon HoaDon { get; set; }
    }
}
