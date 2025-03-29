using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using OnlineClothing.Models;
using System;
using System.Linq;
using System.Threading.Tasks;
namespace OnlineClothing.Controllers.Admin
{
    [Route("Admin/AdminProducts")]
    public class AdminProductsController : Controller
    {
        private readonly ClothingShopPrn222G2Context _context;
        public AdminProductsController(ClothingShopPrn222G2Context context)
        {
            _context = context;
        }
        [HttpGet("Manage")]
        public async Task<IActionResult> Manage(int page = 1, int pageSize = 4, long? categoryId = null, int? statusId = null)
        {
            var categories = await _context.Categories.Select(c => new { c.Id, c.Name }).ToListAsync();
            ViewBag.Categories = new SelectList(categories, "Id", "Name");

            var statusList = new List<SelectListItem>
    {
                new SelectListItem { Value = "1", Text = "Đã duyệt" },
                new SelectListItem { Value = "2", Text = "Chưa duyệt" },
                new SelectListItem { Value = "3", Text = "Bị từ chối" }
    };
            ViewBag.StatusList = new SelectList(statusList, "Value", "Text");

            var query = _context.Products.AsQueryable();

            if (categoryId.HasValue && await _context.Categories.AnyAsync(c => c.Id == categoryId))
            {
                query = query.Where(p => p.CategoryId == categoryId);
            }

            if (statusId.HasValue && (statusId == 1 || statusId == 2 || statusId == 3))
            {
                query = query.Where(p => p.Status == statusId);
            }

            var totalProducts = await query.CountAsync();
            var products = await query
                .Select(p => new AdminProductViewModel
                {
                    ProductId = p.Id,
                    ProductName = p.Name,
                    SellerId = p.SellerId,
                    SellerName = _context.Userinfos.Where(ui => ui.Id == p.SellerId)
                                    .Select(ui => ui.FullName).FirstOrDefault(),
                    CategoryId = p.CategoryId,
                    CategoryName = _context.Categories.Where(c => c.Id == p.CategoryId)
                                    .Select(c => c.Name).FirstOrDefault(),
                    ThumbnailUrl = p.ThumbnailUrl,
                    Description = p.Description,
                    Price = p.Price,
                    Discount = p.Discount,
                    Quantity = p.Quantity,
                    CreatedAt = p.CreateAt,
                    UpdatedAt = p.UpdateAt,
                    Status = p.Status,
                    StatusName = p.Status == 1 ? "Đã duyệt" : p.Status == 2 ? "Chưa duyệt" : "Bị từ chối",
                    RejectionReason = _context.ProductRejectionLogs.Where(r => r.ProductId == p.Id)
                                                    .OrderByDescending(r => r.RejectedAt)
                                                    .Select(r => r.Reason).FirstOrDefault()
                })
                .OrderByDescending(p => p.CreatedAt)
                .Skip((page - 1) * pageSize)
                .Take(pageSize)
                .ToListAsync();

            ViewBag.TotalPages = (int)Math.Ceiling((double)totalProducts / pageSize);
            ViewBag.CurrentPage = page;
            ViewBag.SelectedCategory = categoryId;
            ViewBag.SelectedStatus = statusId;

            return View(products);
        }

        [HttpPost("ApproveProduct")]
        public async Task<IActionResult> ApproveProduct(long productId)
        {
            var product = await _context.Products.FindAsync(productId);
            if (product == null) return NotFound();

            product.Status = 1;
            product.UpdateAt = DateTime.UtcNow;
            await _context.SaveChangesAsync();

            return Json(new { success = true });
        }

        [HttpPost("RejectProduct")]
        public async Task<IActionResult> RejectProduct(long productId, string reason)
        {
            var product = await _context.Products.FindAsync(productId);
            if (product == null) return NotFound();
            product.Status = 3;
            product.UpdateAt = DateTime.UtcNow;

            var existingLog = await _context.ProductRejectionLogs
                                .Where(r => r.ProductId == productId)
                                .OrderByDescending(r => r.RejectedAt)
                                .FirstOrDefaultAsync();
            if (existingLog != null)
            {
                existingLog.Reason = reason; 
                existingLog.RejectedAt = DateTime.UtcNow;
            }
            else
            {
                _context.ProductRejectionLogs.Add(new ProductRejectionLog
                {
                    ProductId = productId,
                    Reason = reason,
                    RejectedAt = DateTime.UtcNow
                });
            }
            await _context.SaveChangesAsync();
            return Json(new { success = true });
        }


    }
}
