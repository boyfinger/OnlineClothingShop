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
                var userRole = HttpContext.Session.GetString("UserRole");
                if (userRole != "SELLER")
                {
                    return RedirectToAction("handleerror", "error", new { statusCode = 403 });
                }

                var product = await _context.Products.FirstOrDefaultAsync(p => p.Id == productId);
                if (product == null)
                {
                    _logger.LogWarning($"Product with id = {productId} not found");
                    return NotFound();
                }

                if (!product.SellerId.Equals(new Guid(userId)))
                {
                    return RedirectToAction("handleerror", "error", new { statusCode = 403 });
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
                ViewBag.Product = product;
                return View(feedbacks);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, $"Failed to get feedbacks of product with id = {productId}");
                return RedirectToAction("handleerror", "error", new { statusCode = 500 });
            }
        }

        [HttpGet]
        public async Task<IActionResult> Create(Guid orderId, long productId)
        {
            var order = await _context.Orders
                .Where(o => o.Id == orderId)
                .Select(o => new { o.Status })
                .FirstOrDefaultAsync();
            if (order == null || order.Status != 4)
            {
                return RedirectToAction("Details", "Orders", new { id = orderId });
            }
            var orderDetail = await _context.OrderDetails
                .Include(od => od.Product)
                .ThenInclude(p => p.Category)
                .FirstOrDefaultAsync(od => od.OrderId == orderId && od.ProductId == productId);

            if (orderDetail == null)
            {
                return NotFound();
            }
            ViewData["Product"] = orderDetail.Product;
            ViewData["orderDetail"] = orderDetail;
            var feedback = new Feedback
            {
                OrderId = orderId,
                ProductId = productId
            };
            return View(feedback);
        }

        [HttpPost]
        public async Task<IActionResult> Create(Feedback feedback)
        {
            var order = await _context.Orders
                .Where(o => o.Id == feedback.OrderId)
                .Select(o => new { o.Status })
                .FirstOrDefaultAsync();
            if (order == null || order.Status != 4 || feedback.Comment == null)
            {
                return RedirectToAction("Details", "Orders", new { id = feedback.OrderId });
            }
            feedback.UserId = Guid.Parse(HttpContext.Session.GetString("UserId"));
            feedback.CreateAt = DateTime.Now;
            _context.Feedbacks.Add(feedback);
            await _context.SaveChangesAsync();
            return RedirectToAction("Details", "Orders", new { id = feedback.OrderId });
        }

        [HttpGet("Details/{orderId}/{productId}")]
        public async Task<IActionResult> Details(Guid orderId, long productId)
        {
            var feedback = await _context.Feedbacks
                .Include(f => f.Product)
                .Include(f => f.User)
                .FirstOrDefaultAsync(f => f.OrderId == orderId && f.ProductId == productId);

            if (feedback == null)
            {
                return NotFound("Không tìm thấy đánh giá.");
            }

            return View(feedback);
        }
    }
}
