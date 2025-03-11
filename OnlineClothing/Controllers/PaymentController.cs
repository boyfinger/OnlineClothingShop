using DotNetEnv;
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

        public MoMoUserInfo ShipInfo;
        
        public PaymentController(ClothingShopPrn222G2Context context)
        {
            _context = context;
        }

        public async Task<IActionResult> Index()
        {
            // Validate user
            var userId = HttpContext.Session.GetString("UserId");
            if (userId == null) return RedirectToAction("Login", "Account");

            // Get cart
            var cart = await _context.Carts
                .Include(c => c.CartDetails)
                .ThenInclude(cd => cd.Product)
                .FirstOrDefaultAsync(c => c.UserId == Guid.Parse(userId));

            // No items in cart == no payment
            if (cart == null)
            {
                return RedirectToAction("Index", "Cart");
            }

            return View();
        }


        [HttpPost]
        public async Task<IActionResult> CheckOutAsync(string FullName, string PhoneNumber, string Address)
        {
            // Validate user
            var userId = HttpContext.Session.GetString("UserId");
            if (userId == null) return RedirectToAction("Login", "Account");


            // Validate shipping info
            if (FullName == null || PhoneNumber == null || Address == null)
            {
                TempData["error"] = "Please fill in shipping infomation";
                return RedirectToAction("Index", "Cart");
            }

            // Get cart
            var cart = await _context.Carts
                .Include(c => c.CartDetails)
                .ThenInclude(cd => cd.Product)
                .FirstOrDefaultAsync(c => c.UserId == Guid.Parse(userId));

            // No items in cart == no payment
            if (cart == null || cart.CartDetails.Count() == 0)
            {
                TempData["error"] = "You must have items in cart to checkout";
                return RedirectToAction("Index", "Cart");
            }

            Env.Load();

            string accessKey = Env.GetString("MOMO_ACCESS_KEY");
            string secretKey = Env.GetString("MOMO_SECRET_KEY");

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
            
            _request.UserInfo = new MoMoUserInfo { Name = FullName, PhoneNumber = PhoneNumber, Address = Address };
            _request.partnerCode = "MOMO";
            _request.redirectUrl = "http://localhost:5222/Payment/Result";
            _request.ipnUrl = "localhost:8080/Payment/Result";
            _request.requestId = _request.orderId;
            _request.requestType = "payWithMethod";
            _request.extraData = "";
            _request.storeId = "Online Clothing Shop";
            _request.autoCapture = true;
            _request.amount = 1000;

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
            var contentsString = await quickPayResponse.Content.ReadAsStringAsync();

            //Console.WriteLine("-----------------------------------------------------------------");
            //Console.WriteLine(contentsString);
            //Console.WriteLine("-----------------------------------------------------------------");

            var response = JsonSerializer.Deserialize<PaymentResult>(contentsString);

            if (response == null)
            {
                TempData["error"] = "Không thể tạo link thanh toán";
                return View("Index");
            }

            if (response.ResultCode == 0)
            {
                Order order = new()
                {
                    Id = Guid.Parse(response.OrderId),
                    CustomerId = Guid.Parse(userId),
                    VoucherId = null,
                    FullName = _request.UserInfo.Name,
                    PhoneNumber = _request.UserInfo.PhoneNumber,
                    Address = _request.UserInfo.Address,
                    Note = "none",
                    OrderDate = DateTime.Now,
                    Status = 1,
                    TotalAmount = (int)_request.amount,
                    CreateAt = DateTime.Now
                };
                await _context.Orders.AddAsync(order);

                // create order details
                foreach (CartDetail cd in cart.CartDetails)
                {
                    OrderDetail od = new ()
                    {
                        OrderId = order.Id,
                        ProductId = cd.ProductId,
                        Quantity = cd.Quantity,
                        UnitPrice = cd.Product.Price,
                        TotalPrice = cd.TotalPrice,
                        Discount = 0
                    };
                    await _context.OrderDetails.AddAsync(od);
                }


                await _context.SaveChangesAsync();
                await ClearCartAsync(cart.Id);
            }
            TempData["message"] = "Giao dịch của bạn đang được xử lý, nếu bạn không tự động được chuyển đến trang thanh toán, vui lòng nhấn vào đường dẫn.";
            TempData["paymentLink"] = response.PayUrl;
            return RedirectToAction("Index");
        }

        [HttpGet]
        public async Task<IActionResult> Result(string orderId, string requestId, string resultCode, string message, string partnerCode, long amount, long responseTime, string payUrl, string shortLink)
        {

            bool exist = Guid.TryParse(orderId, out Guid guid);

            if(!exist)
            {
                TempData["error"] = "Không tìm thấy Id đơn hàng";
                return RedirectToAction("Index");
            }

            Order order = await _context.Orders.FirstOrDefaultAsync(o => o.Id.Equals(guid));
            if (order == null)
            {
                TempData["error"] = "Không tìm thấy Id đơn hàng";
                return RedirectToAction("Index");
            }
            if (resultCode.Equals("0"))
            {
                order.Status = 2; //Confirmed payment
                await _context.SaveChangesAsync();
                TempData["message"] = "Đơn hàng của bạn đã được đặt thành công.";
                TempData["orderId"] = guid;
                return RedirectToAction("Index");
            }
            else
            {
                order.Status = 5; //cancled
                await _context.SaveChangesAsync();
                TempData["message"] = "Đơn hàng của bạn đã bị hủy do quá thời gian thanh toán.";
                TempData["orderId"] = guid;
                return RedirectToAction("Index");
            }
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

        private async Task ClearCartAsync(Guid cartId)
        {
            var cartDetails = await _context.CartDetails
                .Where(cd => cd.CartId == cartId)
                .ToListAsync();

            if (cartDetails.Any())
            {
                foreach (var cartItem in cartDetails)
                {
                    var product = await _context.Products.FindAsync(cartItem.ProductId);
                    if (product != null)
                    {
                        product.Quantity -= cartItem.Quantity; // Subtract the quantity
                        if (product.Quantity < 0) product.Quantity = 0; // Prevent negative stock
                        _context.Products.Update(product);
                    }
                }

                _context.CartDetails.RemoveRange(cartDetails); // Remove cart items
                await _context.SaveChangesAsync(); // Save stock update + cart removal
            }
        }

    }
}
