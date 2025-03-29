using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using OnlineClothing.Models;

namespace OnlineClothing.Controllers
{
    public class SellerOrdersController : Controller
    {
        private readonly ClothingShopPrn222G2Context _context;
        private readonly ILogger<SellerOrdersController> _logger;

        private readonly int pageSize = 3;

        public SellerOrdersController(ClothingShopPrn222G2Context context, ILogger<SellerOrdersController> logger)
        {
            _context = context;
            _logger = logger;
        }

        public async Task<IActionResult> Index(int page = 1)
        {
            try
            {
                var userId = HttpContext.Session.GetString("UserId");
                if (userId == null)
                {
                    return RedirectToAction("Login", "Account");
                }

                var userRole = HttpContext.Session.GetString("UserRole");
                if (userRole != "SELLER")
                {
                    return RedirectToAction("handleerror", "error", new { statusCode = 403 });
                }

                var sellerId = new Guid(userId);

                var query = _context.OrderDetails
                    .Where(od => od.Product.SellerId.Equals(sellerId))
                    .Include(od => od.Product)
                    .Include(od => od.Order)
                        .ThenInclude(o => o.Voucher)
                    .Include(od => od.Order.Customer)
                    .Include(od => od.StatusNavigation)
                    .AsQueryable();

                var totalProducts = await query.CountAsync();
                var totalPages = (int)Math.Ceiling(totalProducts / (double)pageSize);

                var orderDetails = await query
                    .OrderByDescending(od => od.Order.OrderDate)
                    .Skip((page - 1) * pageSize)
                    .Take(pageSize)
                    .ToListAsync();

                ViewBag.CurrentPage = page;
                ViewBag.TotalPages = totalPages;
                return View(orderDetails);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, $"Error while getting orders for seller");
                return RedirectToAction("handleerror", "error", new { statusCode = 500 });
            }
        }

        public async Task<IActionResult> UpdateStatus(long id)
        {
            try
            {
                var userId = HttpContext.Session.GetString("UserId");
                if (userId == null)
                {
                    return RedirectToAction("Login", "Account");
                }

                var sellerId = new Guid(userId);
                var userRole = HttpContext.Session.GetString("UserRole");

                var orderDetail = await _context.OrderDetails.FirstOrDefaultAsync(od => od.Id == id && od.Product.SellerId.Equals(sellerId));

                if (userRole != "SELLER" || orderDetail == null)
                {
                    return RedirectToAction("handleerror", "error", new { statusCode = 403 });
                }

                orderDetail.Status += 1;
                _context.Update(orderDetail);
                await _context.SaveChangesAsync();
                TempData["SuccessMessage"] = "Update status successfully";
                return RedirectToAction(nameof(Index));
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, $"Error while updating status for order detail with id = {id}");
                return RedirectToAction("handleerror", "error", new { statusCode = 500 });
            }
        }
    }
}
