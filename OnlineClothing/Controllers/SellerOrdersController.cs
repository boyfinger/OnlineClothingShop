using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using OnlineClothing.Models;

namespace OnlineClothing.Controllers
{
    public class SellerOrdersController : Controller
    {
        private readonly ClothingShopPrn222G2Context _context;

        private readonly Guid sellerId = new Guid("dde923de-6b2a-4104-a293-6da7aaa68ef3");

        public SellerOrdersController(ClothingShopPrn222G2Context context)
        {
            _context = context;
        }

        public async Task<IActionResult> Index()
        {
            var orderDetails = await _context.OrderDetails
                .Where(od => od.Product.SellerId.Equals(sellerId))
                .Include(od => od.Product)
                .Include(od => od.Order)
                .ToListAsync();

            return View(orderDetails);
        }
    }
}
