using System.Net;
using System.Security.Cryptography;
using System.Text;
using System.Text.Json;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Internal;
using OnlineClothing.Models;
using OnlineClothing.Models.MoMo;
using static Microsoft.EntityFrameworkCore.DbLoggerCategory;

namespace OnlineClothing.Controllers
{
    public class PaymentController : Controller
    {
        private readonly ClothingShopPrn222G2Context _context;
        private static readonly HttpClient client = new HttpClient();
        private CollectionLinkRequest _request = new CollectionLinkRequest();

        public PaymentController(ClothingShopPrn222G2Context context)
        {
            _context = context;
        }

        public async Task<IActionResult> Index()
        {
            var userId = HttpContext.Session.GetString("UserId");
            if (userId == null) return RedirectToAction("Login", "Account");

            var cart = await _context.Carts
                .Include(c => c.CartDetails)
                .ThenInclude(cd => cd.Product)
                .FirstOrDefaultAsync(c => c.UserId == Guid.Parse(userId));

            if (cart == null)
            {
                return RedirectToAction("Index", "Cart");
            }
            _request.orderId = Guid.NewGuid().ToString();
            _request.orderInfo = $"Order [{_request.orderId}]";
            _request.amount = (long)cart.TotalAmount;
            _request.Items = await _context.CartDetails
                .Where(cd => cd.CartId == cart.Id)
                .Include(cd => cd.Product)
                .Select(cd => new MoMoItem
                {
                    Id = cd.ProductId,
                    Name = cd.Product.Name,
                    Price = (long)cd.Product.Price, 
                    Currency = "VND", 
                    Quantity = cd.Quantity,
                    TotalPrice = cd.TotalPrice
                })
                .ToListAsync();
            _request.lang = "vi";

            var userInfo = await _context.Userinfos.FindAsync(Guid.Parse(userId));
            if (userInfo != null)
            {
                _request.UserInfo.Name = userInfo.FullName;
                _request.UserInfo.PhoneNumber = userInfo.PhoneNumber;
                _request.UserInfo.Address = userInfo.Address;
            }
            return View(_request);
        }

        //[HttpPost]
        //public async Task<IActionResult> UpdateOrderDetails(CollectionLinkRequest model)
        //{
        //    if (!ModelState.IsValid)
        //    {
        //        return View("Index", model); 
        //    }

        //    var userId = HttpContext.Session.GetString("UserId");
        //    if (userId == null) return RedirectToAction("Login", "Account");

        //    var userInfo = await _context.Userinfos.FindAsync(Guid.Parse(userId));
        //    if (userInfo == null)
        //    {
        //        userInfo = new Userinfo
        //        {
        //            Id = Guid.Parse(userId),
        //            FullName = model.UserInfo.Name,
        //            PhoneNumber = model.UserInfo.PhoneNumber,
        //            Address = model.UserInfo.Address,
        //        };
        //        _context.Userinfos.Add(userInfo);
        //    }
        //    else
        //    {
        //        userInfo.FullName = model.UserInfo.Name;
        //        userInfo.PhoneNumber = model.UserInfo.PhoneNumber;
        //        userInfo.Address = model.UserInfo.Address;
        //        userInfo.UpdateAt = DateTime.Now;
        //    }

        //    await _context.SaveChangesAsync();

        //    return View("Index");
        //}


        [HttpPost]
        public async Task<IActionResult> CheckOutAsync(string requestJson)
        {
            if (!ModelState.IsValid)
            {
                return View("Index");
            }
            var request = JsonSerializer.Deserialize<CollectionLinkRequest>(requestJson);
            _request = request;
            var userId = HttpContext.Session.GetString("UserId");
            if (userId == null) return RedirectToAction("Login", "Account");

            Guid myuuid = Guid.NewGuid();
            string myuuidAsString = myuuid.ToString();

            string accessKey = "F8BBA842ECF85";
            string secretKey = "K951B6PE1waDMi640xX08PD3vg6EkVlz";

            //CollectionLinkRequest request = new CollectionLinkRequest
            //{
            //    orderInfo = orderInfo,
            //    partnerCode = "MOMO",
            //    redirectUrl = "localhost:8080/Payment/Result",
            //    ipnUrl = "localhost:8080/Payment/Result", 
            //    amount = 1000,
            //    orderId = myuuidAsString,
            //    requestId = myuuidAsString,
            //    requestType = "payWithMethod",
            //    extraData = "",
            //    storeId = "Online Clothing Shop",
            //    autoCapture = true,
            //    lang = "vi"
            //};
            _request.partnerCode = "MOMO";
            _request.redirectUrl = "http://localhost:5222/Payment/Result";
            _request.ipnUrl = "localhost:8080/Payment/Result";
            _request.requestId = _request.orderId;
            _request.requestType = "payWithMethod";
            _request.extraData = "";
            _request.storeId = "Online Clothing Shop";
            _request.autoCapture = true;
            _request.amount = 1000;

            Console.WriteLine("---------------------------------------------------");
            Console.WriteLine("ORDER ID: " + _request.orderId);
            Console.WriteLine("---------------------------------------------------");

            var rawSignature = "accessKey=" + accessKey 
                + "&amount=" + _request.amount 
                + "&extraData=" + _request.extraData 
                + "&ipnUrl=" + _request.ipnUrl 
                + "&orderId=" + _request.orderId 
                + "&orderInfo=" + _request.orderInfo 
                + "&partnerCode=" + _request.partnerCode   
                + "&redirectUrl=" + _request.redirectUrl 
                + "&requestId=" + _request.requestId 
                + "&requestType=" + _request.requestType;
            _request.signature = getSignature(rawSignature, secretKey);

            StringContent httpContent = new StringContent(JsonSerializer.Serialize(_request), System.Text.Encoding.UTF8, "application/json");
            var quickPayResponse = await client.PostAsync("https://test-payment.momo.vn/v2/gateway/api/create", httpContent);
            var contents = await quickPayResponse.Content.ReadAsStringAsync();
            Console.WriteLine("---------------------------------------------------");
            Console.WriteLine(contents);
            Console.WriteLine("---------------------------------------------------");
            using JsonDocument doc = JsonDocument.Parse(contents);
            JsonElement root = doc.RootElement;

            if (root.TryGetProperty("payUrl", out JsonElement payUrlElement))
            {
                string payUrl = payUrlElement.GetString() ?? string.Empty;
                if (!string.IsNullOrEmpty(payUrl))
                {
                    TempData["response"] = contents;
                    return Redirect(payUrl); // Redirect to the payment page
                }
            }
            TempData["mesasge"] = "Failed to create payment link";
            return RedirectToAction("Index");
        }

        [HttpGet]
        public async Task<IActionResult> Result(string orderId, string requestId, string resultCode, string message, string partnerCode, long amount, long responseTime, string payUrl, string shortLink)
        {
            Console.WriteLine("__________________________________________________________");
            Console.WriteLine("An UwU OwO OvO UvU");
            Console.WriteLine("__________________________________________________________");

            var paymentResult = new PaymentResult
            {
                OrderId = orderId,
                RequestId = requestId,
                ResultCode = int.Parse(resultCode),
                Message = message,
                PartnerCode = partnerCode,
                Amount = amount,
                ResponseTime = responseTime,
                PayUrl = payUrl,
                ShortLink = shortLink
            };

            return View(paymentResult);
        }



        private static string getSignature(string text, string key)
        {
            UTF8Encoding encoding = new UTF8Encoding(); 

            byte[] textBytes = encoding.GetBytes(text);
            byte[] keyBytes = encoding.GetBytes(key);

            using HMACSHA256 hash = new HMACSHA256(keyBytes);
            byte[] hashBytes = hash.ComputeHash(textBytes);

            return BitConverter.ToString(hashBytes).Replace("-", "").ToLower();
        }
    }
}
