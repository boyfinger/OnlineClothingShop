using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Http;
using Microsoft.EntityFrameworkCore;
using OnlineClothing.Models;
using System;
using System.Linq;
using System.Threading.Tasks;

namespace OnlineClothing.Controllers
{
    public class OrdersController : Controller
    {
        private readonly ClothingShopPrn222G2Context _context;

        public OrdersController(ClothingShopPrn222G2Context context)
        {
            _context = context;
        }

        [HttpGet]
        public async Task<IActionResult> Index()
        {
            var userId = HttpContext.Session.GetString("UserId");
            if (string.IsNullOrEmpty(userId))
            {
                return RedirectToAction("Login", "Account");
            }

            Console.WriteLine(userId);
            var orders = await _context.Orders
                .Where(o => o.CustomerId.Equals(Guid.Parse(userId)))
                .OrderByDescending(o => o.OrderDate)
                .ToListAsync();
            foreach (var order in orders)
            {
                if (order.Status == 4)
                {
                    var feedbacked = await _context.Feedbacks.FirstOrDefaultAsync(f => f.UserId.Equals(order.CustomerId) && f.OrderId.Equals(order.Id));
                    if (feedbacked == null)
                    {
                        order.CanReview = true;
                    }
                }
            }
            return View(orders);
        }

        public async Task<IActionResult> Details(Guid id)
        {
            var userId = HttpContext.Session.GetString("UserId");
            if (string.IsNullOrEmpty(userId))
            {
                return RedirectToAction("Login", "Account");
            }
            var order = await _context.Orders
                .Include(o => o.OrderDetails)
                    .ThenInclude(od => od.Product)
                .Include(o => o.StatusNavigation)
                .FirstOrDefaultAsync(o => o.Id.Equals(id));
            if (order == null)
            {
                return NotFound();
            }
            var customerId = order.CustomerId;
            if (order.Status == 4)
            {
                foreach (var item in order.OrderDetails)
                {
                    var feedbacked = await _context.Feedbacks
                        .AnyAsync(f => f.UserId == customerId && f.OrderId == order.Id && f.ProductId == item.ProductId);
                    item.Feedbacked = feedbacked;
                }
            }
            else
            {
                foreach (var item in order.OrderDetails)
                {
                    item.Feedbacked = false;
                }
            }
            return View(order);
        }

    }
}
