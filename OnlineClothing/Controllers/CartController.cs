using System.Security.Claims;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using OnlineClothing.Models;

namespace OnlineClothing.Controllers
{
    public class CartController : Controller
    {
        private readonly ClothingShopPrn222G2Context _context;

        public CartController(ClothingShopPrn222G2Context context)
        {
            _context = context;
        }

        public async Task<IActionResult> Index()
        {
            // Get the logged-in user ID (Assuming authentication is used)
            var userId = HttpContext.Session.GetString("UserId");

            if (string.IsNullOrEmpty(userId))
            {
                return RedirectToAction("Login", "Account"); // Redirect if user is not logged in
            }

            // Fetch cart with details
            var cart = await _context.Carts
                .Include(c => c.CartDetails)
                .ThenInclude(cd => cd.Product)
                .FirstOrDefaultAsync(c => c.UserId == Guid.Parse(userId));

            if (cart == null)
            {
                ViewBag.Message = "Your cart is empty.";
                return View();
            }

            return View(cart);
        }

        [HttpPost]
        public async Task<IActionResult> AddToCart(long productId, int quantity)
        {
            var userIdString = HttpContext.Session.GetString("UserId");
            if (string.IsNullOrEmpty(userIdString))
            {
                return Unauthorized();
            }

            Guid userId = Guid.Parse(userIdString);

            // Find or create the user's cart
            var cart = await _context.Carts
                .Include(c => c.CartDetails)
                .FirstOrDefaultAsync(c => c.UserId == userId);

            if (cart == null)
            {
                cart = new Cart
                {
                    Id = Guid.NewGuid(),
                    UserId = userId,
                    TotalAmount = 0,
                    CreateAt = DateTime.UtcNow,
                    UpdateAt = DateTime.UtcNow
                };
                _context.Carts.Add(cart);
                await _context.SaveChangesAsync();
            }

            // Get the product details
            var product = await _context.Products.FindAsync(productId);
            if (product == null)
            {
                return NotFound();
            }

            // Check if the product is already in the cart
            var cartDetail = cart.CartDetails.FirstOrDefault(cd => cd.ProductId == productId);

            if (cartDetail != null)
            {
                // Update quantity and recalculate total price
                cartDetail.Quantity += quantity;
                cartDetail.TotalPrice = cartDetail.Quantity * product.Price;
                cartDetail.UpdateAt = DateTime.UtcNow;
            }
            else
            {
                // Add new product to cart
                cartDetail = new CartDetail
                {
                    CartId = cart.Id,
                    ProductId = productId,
                    Quantity = quantity,
                    TotalPrice = quantity * product.Price,
                    CreateAt = DateTime.UtcNow,
                    UpdateAt = DateTime.UtcNow
                };
                _context.CartDetails.Add(cartDetail);
            }

            // Recalculate cart's total amount correctly
            cart.TotalAmount = cart.CartDetails.Sum(cd => cd.TotalPrice);
            cart.UpdateAt = DateTime.UtcNow;

            await _context.SaveChangesAsync();

            return Ok();
        }

        [HttpPost]
        public async Task<IActionResult> Remove(long productId)
        {
            var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
            if (string.IsNullOrEmpty(userId)) return RedirectToAction("Login", "Account");

            var cart = await _context.Carts
                .Include(c => c.CartDetails)
                .FirstOrDefaultAsync(c => c.UserId == Guid.Parse(userId));

            if (cart != null)
            {
                var item = cart.CartDetails.FirstOrDefault(cd => cd.ProductId == productId);
                if (item != null)
                {
                    _context.CartDetails.Remove(item);
                    await _context.SaveChangesAsync();
                }
            }

            return RedirectToAction("Index");
        }
    }
}
