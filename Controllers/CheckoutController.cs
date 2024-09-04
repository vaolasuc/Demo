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
            var deliveryAddress = "1234 Main St";
            var totalAmount = data?["amount"]?.ToString();
            var productIdentifiers = "1";

            if (deliveryAddress == null || totalAmount == null || productIdentifiers == null)
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
            var response = new
            {
                Id = orderId
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
                            var hoadong = _db.HoaDon.Where(hd => hd.ApplicationUserId == claim.Value).ToList();

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
