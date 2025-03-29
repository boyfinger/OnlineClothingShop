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
        [HttpPost("ProcessReport")]
        public async Task<IActionResult> ProcessReport(long id) // Chuyển sang trạng thái "Đang xử lý" (status 2)
        {
            try
            {
                var report = await _context.Reports.FirstOrDefaultAsync(r => r.Id == id);
                if (report == null)
                {
                    return Json(new { success = false, message = "Không tìm thấy đơn khiếu nại" });
                }

                report.Status = 2; // Đang xử lý
                report.UpdateAt = DateTime.Now;
                await _context.SaveChangesAsync();

                return Json(new
                {
                    success = true,
                    message = "Đã chuyển sang trạng thái đang xử lý",
                    newStatus = 2,
                    newStatusText = "Đang xử lý"
                });
            }
            catch (Exception ex)
            {
                return Json(new { success = false, message = $"Lỗi hệ thống: {ex.Message}" });
            }
        }

        [HttpPost("CompleteReport")]
        public async Task<IActionResult> CompleteReport(long id) // Hoàn thành (status 3)
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

                report.Status = 3; // Đã giải quyết
                report.UpdateAt = DateTime.Now;

                if (report.Product != null)
                {
                    report.Product.Status = 1; // Đánh dấu sản phẩm là đã xử lý
                    report.Product.UpdateAt = DateTime.Now;
                }

                await _context.SaveChangesAsync();

                return Json(new
                {
                    success = true,
                    message = "Đã hoàn thành xử lý đơn khiếu nại",
                    newStatus = 3,
                    newStatusText = "Đã giải quyết"
                });
            }
            catch (Exception ex)
            {
                return Json(new { success = false, message = $"Lỗi hệ thống: {ex.Message}" });
            }
        }

        [HttpPost("RejectReport")]
        public async Task<IActionResult> RejectReport(long id) // Từ chối (status 4)
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

                report.Status = 4; // Từ chối
                report.UpdateAt = DateTime.Now;

                if (report.Product != null)
                {
                    report.Product.Status = 1; // Khôi phục trạng thái sản phẩm
                    report.Product.UpdateAt = DateTime.Now;
                }

                await _context.SaveChangesAsync();

                return Json(new
                {
                    success = true,
                    message = "Đã từ chối đơn khiếu nại",
                    newStatus = 4,
                    newStatusText = "Từ chối"
                });
            }
            catch (Exception ex)
            {
                return Json(new { success = false, message = $"Lỗi hệ thống: {ex.Message}" });
            }
        }
    }
}
