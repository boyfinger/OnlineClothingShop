using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using OnlineClothing.Models;

namespace OnlineClothing.Controllers
{
    public class FeedbackController : Controller
    {
        private readonly ClothingShopPrn222G2Context _context;
        public FeedbackController(ClothingShopPrn222G2Context context)
        {
            _context = context;
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
