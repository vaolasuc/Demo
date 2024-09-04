// using Demo.Models;
// using Microsoft.AspNetCore.Mvc;
// using PayPalCheckoutSdk.Core;
// using PayPalCheckoutSdk.Orders;
// using PayPalHttp;
// using System.Threading.Tasks;

// namespace Demo.Controllers
// {
//     public class PayPalController : Controller
//     {
//         private readonly HttpClient _client;

//         public PayPalController()
//         {
//             _client = new PayPalHttpClient(new PayPalEnvironment.Sandbox(
//                 PaypalConfiguration.ClientId,
//                 PaypalConfiguration.ClientSecret));
//         }

//         public async Task<IActionResult> CreateOrder()
//         {
//             var orderRequest = new OrdersCreateRequest();
//             orderRequest.Prefer("return=representation");
//             orderRequest.RequestBody(BuildRequestBody());

//             var response = await _client.Execute(orderRequest);
//             var result = response.Result<PayPalCheckoutSdk.Orders.Order>();

//             return Json(result);
//         }

//         private OrderRequest BuildRequestBody()
//         {
//             return new OrderRequest()
//             {
//                 CheckoutPaymentIntent = "CAPTURE",
//                 PurchaseUnits = new List<PurchaseUnitRequest>
//                 {
//                     new PurchaseUnitRequest
//                     {
//                         AmountWithBreakdown = new AmountWithBreakdown
//                         {
//                             CurrencyCode = "USD",
//                             Value = "100.00"
//                         }
//                     }
//                 },
//                 ApplicationContext = new ApplicationContext
//                 {
//                     BrandName = "Demo",
//                     LandingPage = "BILLING",
//                     UserAction = "PAY_NOW",
//                     ReturnUrl = "https://example.com/return",
//                     CancelUrl = "https://example.com/cancel"
//                 }
//             };
//         }
//     }
// }