using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using OnlineClothing.Models;

namespace OnlineClothing.Controllers
{
    public class FeedbackController : Controller
    {
        private readonly ClothingShopPrn222G2Context _context;
        private readonly ILogger<FeedbackController> _logger;

        private readonly int pageSize = 4;

        public FeedbackController(ClothingShopPrn222G2Context context, ILogger<FeedbackController> logger)
        {
            _context = context;
            _logger = logger;
        }

        public async Task<IActionResult> Index(int productId, int page = 1)
        {
            try
            {
                string? userId = HttpContext.Session.GetString("UserId");
                if (userId == null)
                {
                    return RedirectToAction("login", "account");
                }
                var roles = await _context.UserRoles
                    .Where(ur => ur.RoleId == 2 && ur.UserId.Equals(new Guid(userId)))
                    .ToListAsync();
                if (roles == null || roles.Count == 0)
                {
                    ViewData["StatusCode"] = 403;
                    @ViewData["ErrorMessage"] = "You don't have the permission to access this page.";
                    return RedirectToAction("error", "home");
                }

                var product = await _context.Products.FirstOrDefaultAsync(p => p.Id == productId);
                if (product == null)
                {
                    _logger.LogWarning($"Product with id = {productId} not found");
                    return NotFound();
                }

                if (!product.SellerId.Equals(new Guid(userId)))
                {
                    ViewData["StatusCode"] = 403;
                    @ViewData["ErrorMessage"] = "You don't have the permission to access this page.";
                    return RedirectToAction("error", "home");
                }

                var query = _context.Feedbacks
                    .Where(f => f.ProductId == productId)
                    .Include(f => f.Product)
                    .Include(f => f.User)
                    .Include(f => f.User.Userinfo)
                    .AsQueryable();

                var totalProducts = await query.CountAsync();
                var totalPages = (int)Math.Ceiling(totalProducts / (double)pageSize);

                var feedbacks = await query
                    .Skip((page - 1) * pageSize)
                    .Take(pageSize)
                    .ToListAsync();

                ViewBag.CurrentPage = page;
                ViewBag.TotalPages = totalPages;
                ViewBag.Product = await _context.Products.FirstOrDefaultAsync(p => p.Id == productId);
                return View(feedbacks);
            }
            catch (Exception ex)
            {
                _logger.LogError($"Failed to get feedbacks of product with id = {productId}", ex);
                return RedirectToAction("error", "home");
            }
        }
    }
}
