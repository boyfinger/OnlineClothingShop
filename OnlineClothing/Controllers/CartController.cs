﻿using System.Security.Claims;
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
            var userId = HttpContext.Session.GetString("UserId");

            if (string.IsNullOrEmpty(userId))
            {
                return RedirectToAction("Login", "Account"); 
            }

            var cart = await _context.Carts
                .Include(c => c.CartDetails)
                .ThenInclude(cd => cd.Product)
                .FirstOrDefaultAsync(c => c.UserId == Guid.Parse(userId));

            if (cart == null || cart.CartDetails.Count == 0)
            {
                ViewBag.Message = "Your cart is empty.";
                return View();
            }

            var userinfo = await _context.Userinfos.FirstOrDefaultAsync(i => i.Id.Equals(Guid.Parse(userId)));
            if (userinfo != null)
            {
                ViewBag.FullName = userinfo.FullName;
                ViewBag.PhoneNumber = userinfo.PhoneNumber;
                ViewBag.Address = userinfo.Address;
            }

            var vouchers = await _context.Vouchers
                .Include(v => v.TypeNavigation)
                .Include(v => v.VoucherUsages)
                .Where(v => v.Status == 1
                    && v.EndDate.HasValue
                    && v.EndDate > DateTime.Now
                    && !v.VoucherUsages.Any(uv => uv.UserId == Guid.Parse(userId))) 
                .ToListAsync();

            TempData["vouchers"] = vouchers;

            // validate cart items (check for discounts (if expired)
            int totalAmount = 0;
            foreach (CartDetail item in cart.CartDetails)
            {
                int totalPrice = 0;
                Product p = await _context.Products.FirstOrDefaultAsync(p => p.Id ==  item.ProductId);
                if (p != null)
                {
                    totalPrice += (item.Quantity * (p.Price - p.Price * p.Discount / 100)) ?? 0;
                }
                item.TotalPrice = totalPrice;
                totalAmount += totalPrice;
            }

            cart.TotalAmount = totalAmount;
            await _context.SaveChangesAsync();
 
            return View(cart);
        }

        [HttpPost]
        public async Task<IActionResult> AddToCart(long productId, int quantity)
        {
            var userId = HttpContext.Session.GetString("UserId");

            if (string.IsNullOrEmpty(userId))
            {
                if (Request.Headers["X-Requested-With"] == "XMLHttpRequest")
                {
                    return Json(new { message = "You have to log in to add to cart" });
                }
                else
                {
                    return RedirectToAction("Login", "Account");
                }
            }

            // Find or create the user's cart
            var cart = await _context.Carts
                .Include(c => c.CartDetails)
                .FirstOrDefaultAsync(c => c.UserId == Guid.Parse(userId));

            if (cart == null)
            {
                cart = new Cart
                {
                    Id = Guid.NewGuid(),
                    UserId = Guid.Parse(userId),
                    TotalAmount = 0,
                    CreateAt = DateTime.UtcNow,
                    UpdateAt = DateTime.UtcNow
                };
                _context.Carts.Add(cart);
                await _context.SaveChangesAsync();
            }

            var product = await _context.Products.FindAsync(productId);
            if (product == null)
            {
                return NotFound();
            }

            if (quantity > product.Quantity)
            {
                return Json(new { message = "Product quantity exceeded available amount." });
            }
            var cartDetail = cart.CartDetails.FirstOrDefault(cd => cd.ProductId == productId);

            if (cartDetail != null)
            {
                cartDetail.Quantity += quantity;
                cartDetail.TotalPrice = (cartDetail.Quantity * (product.Price - product.Price*product.Discount/100))?? 0;
                cartDetail.UpdateAt = DateTime.UtcNow;
            }
            else
            {
                cartDetail = new CartDetail
                {
                    CartId = cart.Id,
                    ProductId = productId,
                    Quantity = quantity,
                    TotalPrice = (quantity * (product.Price - product.Price * product.Discount / 100)) ?? 0,
                    CreateAt = DateTime.UtcNow,
                    UpdateAt = DateTime.UtcNow
                };
                _context.CartDetails.Add(cartDetail);
            }

            cart.TotalAmount = cart.CartDetails.Sum(cd => cd.TotalPrice);
            cart.UpdateAt = DateTime.UtcNow;

            await _context.SaveChangesAsync();

            // Logic to add product to cart
            return Json(new { message = "Product added to cart successfully!" });
        }

        [HttpPost]
        public async Task<IActionResult> Remove(long productId)
        {
            var userId = HttpContext.Session.GetString("UserId");
            if (string.IsNullOrEmpty(userId))
            {
                return Json(new { success = false, redirectUrl = Url.Action("Login", "Account") });
            }

            var cart = await _context.Carts
                .Include(c => c.CartDetails)
                .FirstOrDefaultAsync(c => c.UserId == Guid.Parse(userId));

            if (cart != null)
            {
                var item = cart.CartDetails.FirstOrDefault(cd => cd.ProductId == productId);
                if (item != null)
                {
                    _context.CartDetails.Remove(item);
                    cart.TotalAmount -= item.TotalPrice;
                    await _context.SaveChangesAsync();

                    // Check if the cart is empty
                    bool isCartEmpty = !cart.CartDetails.Any();

                    return Json(new
                    {
                        success = true,
                        totalAmount = string.Format("{0:N0} VND", cart.TotalAmount),
                        isCartEmpty = isCartEmpty
                    });
                }
            }

            return Json(new { success = false });
        }


        [HttpPost]
        public async Task<IActionResult> UpdateQuantity(long productId, int quantity)
        {
            var userId = HttpContext.Session.GetString("UserId");
            if (string.IsNullOrEmpty(userId))
            {
                return Json(new { success = false, redirectUrl = Url.Action("Login", "Account") });
            }

            var cart = await _context.Carts
                .Include(c => c.CartDetails)
                .ThenInclude(cd => cd.Product) // Include product to access its price
                .FirstOrDefaultAsync(c => c.UserId == Guid.Parse(userId));

            if (cart == null)
            {
                return Json(new { success = false });
            }

            var item = cart.CartDetails.FirstOrDefault(cd => cd.ProductId == productId);
            if (item == null || quantity <= 0)
            {
                return Json(new { success = false });
            }

            // Update quantity and total price of the item
            item.Quantity = quantity;
            //item.TotalPrice = (item.Quantity * item.Product.Price) ?? 0;
            item.TotalPrice = quantity * (item.Product.Price - item.Product.Price * item.Product.Discount / 100) ?? 0;
            item.UpdateAt = DateTime.UtcNow;
            await _context.SaveChangesAsync();
            // Update total cart amount
            cart.TotalAmount = cart.CartDetails.Sum(cd => cd.TotalPrice);
            cart.UpdateAt = DateTime.UtcNow;

            await _context.SaveChangesAsync();

            return Json(new
            {
                success = true,
                newTotalPrice = string.Format("{0:N0} VND", item.TotalPrice),
                totalAmount = string.Format("{0:N0} VND", cart.TotalAmount)
            });
        }


    }
}
