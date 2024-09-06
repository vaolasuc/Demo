using Demo.Models;
using Microsoft.AspNetCore.Mvc;
using System.Collections.Specialized;
using System.Security.Cryptography;
using System.Text.Json.Nodes;
using System.Text;
using Demo.Data;
using Demo.Data.Migrations;
using Microsoft.EntityFrameworkCore;
using System.Security.Claims;
using HoaDonModel = Demo.Models.HoaDon;

namespace Demo.Controllers
{
    public class CheckoutController : Controller
    {
        private string PaypalClientId { get; set; } = "";
        private string PaypalSecret { get; set; } = "";
        private string PaypalUrl { get; set; } = "";
        private readonly ApplicationDbContext _db;

        public CheckoutController(ApplicationDbContext db, IConfiguration configuration)
        {
            _db = db;
            PaypalClientId = configuration["PaypalSettings:ClientId"];
            PaypalSecret = configuration["PaypalSettings:Secret"];
            PaypalUrl = configuration["PaypalSettings:Url"];
        }

        public IActionResult Index()
        {
            ViewBag.PaypalClientId = PaypalClientId;
            return View();
        }

        public string GetPaypalClientId()
        {
            return PaypalClientId ?? "";
        }

        [HttpPost]
        public async Task<JsonResult> CreateOrder([FromBody] JsonObject data)
        {

            var deliveryAddress = data?["address"];
            var totalAmount = data?["amount"]?.ToString();
            var productIdentifiers = "1";
            var deliveryPhoneNumber = data?["phone_number"];
            var deliveryName = data?["name"];

            if (deliveryAddress == null || deliveryPhoneNumber == null || totalAmount == null || deliveryName == null)
            {
                return new JsonResult(new {Id = ""});
            }

            // create the request body
            JsonObject requestBody = new JsonObject();
            requestBody["intent"] = "CAPTURE";

            JsonObject amount = new JsonObject();
            amount["currency_code"] = "USD";
            amount["value"] = totalAmount;

            JsonObject purchaseUnits = new JsonObject();
            purchaseUnits["amount"] = amount;

            JsonArray purchaseUnitsArray = new JsonArray();
            purchaseUnitsArray.Add(purchaseUnits);

            requestBody["purchase_units"] = purchaseUnitsArray;

            // send the request to PayPal
            string accessToken = await GetPaypalAccessToken();
            string url = PaypalUrl + "/v2/checkout/orders";

            string orderId = "";
            using (var client = new HttpClient())
            {
                client.DefaultRequestHeaders.Add("Authorization", "Bearer " + accessToken);
                client.DefaultRequestHeaders.Add("Accept", "application/json");
                client.DefaultRequestHeaders.Add("Accept-Language", "en_US");

                var requestMessage = new HttpRequestMessage(HttpMethod.Post, url);
                requestMessage.Content = new StringContent(requestBody.ToString(), Encoding.UTF8, "application/json");

                var responseTask = client.SendAsync(requestMessage);
                responseTask.Wait();

                var result = responseTask.Result;

                if (result.IsSuccessStatusCode)
                {
                    var readTask = result.Content.ReadAsStringAsync();
                    readTask.Wait();

                    var json = JsonNode.Parse(readTask.Result);

                    if (json != null)
                    {
                        orderId = json["id"]!.ToString() ?? "";


                        // Save the order ID to the database
                        
                    }
                }
            }

            // Handle form submission


            //lấy thông tin tài khoản
            var identity = (ClaimsIdentity)User.Identity;
            var claim = identity.FindFirst(ClaimTypes.NameIdentifier);

            GioHangViewModel giohang = new GioHangViewModel()
            {
                DsGioHang = _db.GioHang.Include("SanPham").Where(gh => gh.ApplicationUserId == claim.Value).ToList(),
                HoaDon = new HoaDonModel()
            };
            foreach (var item in giohang.DsGioHang)
            {
                //TÍNH TIỀN THEO SỐ LƯỢNG SẢN PHẨM
                item.ProductPrice = item.Quantity * item.SanPham.Price;
                // Tính tổng số tiền trong giỏ hàng
                giohang.HoaDon.Total += item.ProductPrice;
            }
            //cập nhật thông tin danh sách giỏ hàng và hóa đơn
            //giohang.DsGioHang = _db.GioHang.Include("SanPham").Where(gh => gh.ApplicationUserId == claim.Value).ToList();
            giohang.HoaDon.ApplicationUserId = claim.Value;
            giohang.HoaDon.OrderDate = DateTime.Now;
            giohang.HoaDon.OrderStatus = "Đang Xác Nhận";
            giohang.HoaDon.Address = deliveryAddress.ToString();
            giohang.HoaDon.Name = deliveryName.ToString();
            giohang.HoaDon.PhoneNumber = deliveryPhoneNumber.ToString();
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
            var response = new
            {
                Id = giohang.HoaDon.Id
            };

            return new JsonResult(response);
            
        }

        [HttpPost]
        public async Task<JsonResult> CompletedOrder([FromBody] JsonObject data)
        {
            if (data == null || data["orderID"] == null) return new JsonResult("error");

            var orderId = data["orderID"]!.ToString();

            //
            string accessToken = await GetPaypalAccessToken();
            string url = PaypalUrl + "/v2/checkout/orders/" + orderId + "/capture";

            using (var client = new HttpClient())
            {
                client.DefaultRequestHeaders.Add("Authorization", "Bearer " + accessToken);

                var requestMessage = new HttpRequestMessage(HttpMethod.Post, url);
                requestMessage.Content = new StringContent("", Encoding.UTF8, "application/json");

                var responseTask = client.SendAsync(requestMessage);
                responseTask.Wait();

                var result = responseTask.Result;

                if (result.IsSuccessStatusCode)
                {
                    var readTask = result.Content.ReadAsStringAsync();
                    readTask.Wait();

                    var json = JsonNode.Parse(readTask.Result);

                    if (json != null)
                    {
                        string paypalOrderStatus = json["status"]?.ToString() ?? "";

                        if(paypalOrderStatus == "COMPLETED")
                        {
                            // Save the order ID to the database

                            var identity = (ClaimsIdentity)User.Identity;
                            var claim = identity.FindFirst(ClaimTypes.NameIdentifier);
                            //cập nhật thông tin danh sách giỏ hàng và hóa đơn
                            var hoadong = _db.HoaDon.Where(hd => hd.Id.ToString() == orderId).ToList();

                            hoadong[0].OrderStatus = "Đã thanh toán";

                            _db.SaveChanges();
                            return new JsonResult("success");
                        }



                    }
                }
            }

            var response = new
            {
                Id = orderId
            };

            return new JsonResult(response);
        }

        [HttpPost]
        public async Task<JsonResult> CancelOrder([FromBody] JsonObject data)
        {
            if (data == null || data["orderID"] == null) return new JsonResult("error");

            var orderId = data["orderID"]!.ToString();
           

            var identity = (ClaimsIdentity)User.Identity;
            var claim = identity.FindFirst(ClaimTypes.NameIdentifier);
                            //cập nhật thông tin danh sách giỏ hàng và hóa đơn
            var hoadong = _db.HoaDon.Where(hd => hd.Id.ToString() == orderId).ToList();

            hoadong[0].OrderStatus = "Đã hủy";

            _db.SaveChanges();
            return new JsonResult("success");
                      
        }

        [HttpPost]
        public async Task<JsonResult> ErrorOrder([FromBody] JsonObject data)
        {
            if (data == null || data["orderID"] == null) return new JsonResult("error");

            var orderId = data["orderID"]!.ToString();


            var identity = (ClaimsIdentity)User.Identity;
            var claim = identity.FindFirst(ClaimTypes.NameIdentifier);
            //cập nhật thông tin danh sách giỏ hàng và hóa đơn
            var hoadong = _db.HoaDon.Where(hd => hd.Id.ToString() == orderId).ToList();

            hoadong[0].OrderStatus = "Lỗi";

            _db.SaveChanges();
            return new JsonResult("success");

        }

        public async Task<string> Token()
        {
            return await GetPaypalAccessToken();
        }

        private async Task<string> GetPaypalAccessToken()
        {
            string accessToken = "";

            string url = PaypalUrl + "/v1/oauth2/token";

            using (var client = new HttpClient())
            {
                string credentials64 =
                    Convert.ToBase64String(Encoding.UTF8.GetBytes(PaypalClientId + ":" + PaypalSecret));

                client.DefaultRequestHeaders.Add("Authorization", "Basic " + credentials64);

                var requestMessage = new HttpRequestMessage(HttpMethod.Post, url);

                requestMessage.Content = new StringContent("grant_type=client_credentials", null, "application/x-www-form-urlencoded");

                var result = await client.SendAsync(requestMessage);


                if (result.IsSuccessStatusCode)
                {
                    var readTask = await result.Content.ReadAsStringAsync();

                    var json = JsonNode.Parse(readTask);

                    if (json != null)
                    {
                        accessToken = json["access_token"]!.ToString() ?? "";

                    }

                }
            }

            return accessToken;
        }
    }
}
