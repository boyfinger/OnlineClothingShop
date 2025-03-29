using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using OnlineClothing.Models;

namespace OnlineClothing.Controllers.Admin
{
    [Route("Admin/AdminReports")]
    public class AdminReportsController : Controller
    {
        private readonly ClothingShopPrn222G2Context _context;
        public AdminReportsController(ClothingShopPrn222G2Context context)
        {
            _context = context;
        }
        [HttpGet("Manage")]
        public async Task<IActionResult> Manage(int page = 1, int pageSize = 4, int? statusId = null)
        {
            // Danh sách trạng thái
            var statusList = new List<SelectListItem>
    {
        new SelectListItem { Value = "1", Text = "Chờ xử lý" },
        new SelectListItem { Value = "2", Text = "Đang xử lý" },
        new SelectListItem { Value = "3", Text = "Đã giải quyết" },
        new SelectListItem { Value = "4", Text = "Từ chối" }
    };
            ViewBag.StatusList = new SelectList(statusList, "Value", "Text");

            var query = _context.Reports.AsQueryable();

            // Lọc theo status
            if (statusId.HasValue)
            {
                query = query.Where(r => r.Status == statusId);
            }

            var totalReports = await query.CountAsync();
            var reports = await query.Include(r => r.User)
                .Include(r => r.Product)
                .ThenInclude(p => p.Category)
                .Select(r => new AdminReportViewModel
                {
                    Id = r.Id,
                    ProductId = r.ProductId,
                    UserName = r.User.UserName,
                    Email = r.User.Email,
                    ProductName = r.Product.Name,
                    CategoryName = r.Product.Category.Name,
                    ThumbnailUrl = r.Product.ThumbnailUrl,
                    ProductDescription = r.Product.Description,
                    Price = r.Product.Price,
                    Discount = r.Product.Discount,
                    Quantity = r.Product.Quantity,
                    Reason = r.Reason,
                    Status = r.Status,
                    CreateAt = r.CreateAt,
                    StatusName = r.Status == 1 ? "Chờ xử lý" :
                                 r.Status == 2 ? "Đang xử lý" :
                                 r.Status == 3 ? "Đã giải quyết" : "Từ chối"
                })
                .OrderByDescending(r => r.CreateAt)
                .Skip((page - 1) * pageSize)
                .Take(pageSize)
                .ToListAsync();

            ViewBag.TotalPages = (int)Math.Ceiling((double)totalReports / pageSize);
            ViewBag.CurrentPage = page;
            ViewBag.SelectedStatus = statusId;

            return View(reports);
        }
        [HttpPost("ApproveReport")]
        public async Task<IActionResult> ApproveReport(long id) // id ở đây là reportId
        {
            try
            {
                // 1. Tìm report trong database
                var report = await _context.Reports
                    .Include(r => r.Product) // Load thông tin sản phẩm liên quan nếu cần
                    .FirstOrDefaultAsync(r => r.Id == id);

                if (report == null)
                {
                    return Json(new { success = false, message = "Không tìm thấy đơn khiếu nại" });
                }

 
                report.Status = 3; 
                report.UpdateAt = DateTime.Now;

                if (report.Product != null)
                {
                    report.Product.Status = 3; 
                    report.Product.UpdateAt = DateTime.Now;
                }

                await _context.SaveChangesAsync();

                return Json(new
                {
                    success = true,
                    message = "Đã chấp thuận đơn khiếu nại thành công",
                    newStatus = 3,
                    newStatusText = "Đã chấp thuận"
                });
            }
            catch (Exception ex)
            {
                return Json(new
                {
                    success = false,
                    message = $"Lỗi hệ thống: {ex.Message}"
                });
            }
        }

        [HttpPost("RejectReport")]
        public async Task<IActionResult> RejectReport(long id) 
        {
            try
            {
                
                var report = await _context.Reports
                    .Include(r => r.Product) 
                    .FirstOrDefaultAsync(r => r.Id == id);

                if (report == null)
                {
                    return Json(new { success = false, message = "Không tìm thấy đơn khiếu nại" });
                }

                report.Status = 4; 
                report.UpdateAt = DateTime.Now;

                if (report.Product != null)
                {
                    report.Product.Status = 1; 
                    report.Product.UpdateAt = DateTime.Now;

                }

                await _context.SaveChangesAsync();

                return Json(new
                {
                    success = true,
                    message = "Đã từ chối đơn khiếu nại và cập nhật trạng thái sản phẩm",
                    newStatus = 4,
                    newStatusText = "Đã từ chối"
                });
            }
            catch (Exception ex)
            {
                return Json(new
                {
                    success = false,
                    message = $"Lỗi hệ thống: {ex.Message}"
                });
            }
        }
    }
}
