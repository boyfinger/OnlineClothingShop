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

        // GET: Orders/Index
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

            return View(orders);
        }

        // GET: Orders/Details/5
        [HttpGet]
        public async Task<IActionResult> Details(Guid id)
        {
            var order = await _context.Orders
                .Include(o => o.OrderDetails)
                    .ThenInclude(od => od.Product)
                .Include(o => o.StatusNavigation)
                .FirstOrDefaultAsync(o => o.Id.Equals(id));

            if (order == null)
            {
                return NotFound();
            }

            return View(order);
        }
    }
}
