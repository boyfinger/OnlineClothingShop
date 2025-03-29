using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using OnlineClothing.Models;
using OnlineClothing.Models.ViewModelsOfAdminDashboard;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Threading.Tasks;

namespace OnlineClothing.Controllers.AdminController
{
    [Route("Admin/AdminDashboard")]
    public class AdminDashboardController : Controller
    {
        private readonly ClothingShopPrn222G2Context _context;

        public AdminDashboardController(ClothingShopPrn222G2Context context)
        {
            _context = context;
        }

        [HttpGet("Dashboard")]
        public async Task<IActionResult> DashboardAsync()
        {
            var userRole = HttpContext.Session.GetString("UserRole");
            if (string.IsNullOrEmpty(userRole) || userRole != "ADMIN")
            {
                return RedirectToAction("AdminLogin", "Account");
            }

            // Thống kê tổng quan
            ViewBag.TotalUsers = await _context.Users.CountAsync();
            ViewBag.TotalReports = await _context.Reports.CountAsync();
            ViewBag.TotalShops = await _context.Users
                .Include(u => u.UserRoles)
                .CountAsync(u => u.UserRoles.Any(ur => ur.RoleId == 2));
            ViewBag.TotalProductUnapprove = await _context.Products.CountAsync(p => p.Status == 2);

            // Dữ liệu cho biểu đồ
            await LoadChartData();

            return View();
        }

        private async Task LoadChartData()
        {
            // Lấy danh sách seller cho dropdown
            ViewBag.Sellers = await GetSellers();

            // Dữ liệu cho biểu đồ so sánh doanh thu seller
            ViewBag.SellerRevenueComparison = await GetSellerRevenueComparison();

            // Dữ liệu doanh thu 3 tháng gần nhất
            ViewBag.RevenueTrend = GetRevenueTrend();
        }

        private async Task<List<dynamic>> GetSellers()
        {
            return await _context.Users
                .Include(u => u.UserRoles)
                .Where(u => u.UserRoles.Any(ur => ur.RoleId == 2))
                .Select(u => new
                {
                    Id = u.Id.ToString(), 
                    Name = u.UserName
                })
                .ToListAsync<dynamic>();
        }

        private async Task<List<SellerRevenueVM>> GetSellerRevenueComparison()
        {
            // Lấy doanh thu theo seller thông qua sản phẩm họ bán
            return await _context.Users
                .Where(u => u.UserRoles.Any(ur => ur.RoleId == 2)) // Chỉ lấy user có role seller
                .Select(seller => new SellerRevenueVM
                {
                    SellerId = seller.Id,
                    SellerName = seller.UserName,
                    Revenue = _context.Products
                        .Where(p => p.SellerId == seller.Id)
                        .SelectMany(p => p.OrderDetails)
                        .Sum(od => od.TotalPrice),
                    OrderCount = _context.Products
                        .Where(p => p.SellerId == seller.Id)
                        .SelectMany(p => p.OrderDetails)
                        .Count()
                })
                .OrderByDescending(x => x.Revenue)
                .Take(10)
                .ToListAsync();
        }

        private List<RevenueTrendVM> GetRevenueTrend()
        {
            var threeMonthsAgo = DateTime.Now.AddMonths(-3);

            return _context.Orders
                .Where(o => o.OrderDate != null && o.OrderDate >= threeMonthsAgo)
                .AsEnumerable()
                .GroupBy(o => new
                {
                    Year = o.OrderDate.Value.Year,
                    Month = o.OrderDate.Value.Month
                })
                .Select(g => new RevenueTrendVM
                {
                    Period = $"{g.Key.Month:00}/{g.Key.Year}",
                    Revenue = g.Sum(o => o.TotalAmount)
                })
                .OrderBy(x => x.Period)
                .ToList();
        }

        [HttpPost("GetSellerRevenueData")]
        public async Task<IActionResult> GetSellerRevenueData(string sellerId)
        {
            try
            {
                if (string.IsNullOrEmpty(sellerId))
                {
                    return BadRequest("Seller ID is required");
                }

                var currentYear = DateTime.Now.Year;
                var startDate = new DateTime(currentYear, 1, 1);
                var endDate = new DateTime(currentYear, 12, 31);

                var revenueData = await _context.OrderDetails
                    .Include(od => od.Order)
                    .Include(od => od.Product)
                    .Where(od => od.Product.SellerId.ToString() == sellerId &&
                                od.Order.OrderDate >= startDate &&
                                od.Order.OrderDate <= endDate &&
                                od.Order.Status == 4) // Chỉ lấy đơn hàng đã hoàn thành
                    .GroupBy(od => od.Order.OrderDate.Value.Month) // Nhóm theo tháng
                    .Select(g => new MonthlyRevenueVM
                    {
                        Month = g.Key,
                        MonthName = CultureInfo.CurrentCulture.DateTimeFormat.GetMonthName(g.Key),
                        Revenue = g.Sum(od => od.TotalPrice)
                    })
                    .OrderBy(x => x.Month)
                    .ToListAsync();

                return Json(revenueData);
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { error = ex.Message });
            }
        }
        [HttpGet("GetTopProducts")]
        public async Task<IActionResult> GetTopProducts(Guid sellerId, int count = 5)
        {
            var topProducts = await _context.Products
                .Where(p => p.SellerId == sellerId)
                .SelectMany(p => p.OrderDetails)
                .GroupBy(od => new
                {
                    od.Product.Id,
                    od.Product.Name
                })
                .Select(g => new TopProductVM
                {
                    ProductId = g.Key.Id,
                    ProductName = g.Key.Name,
                    TotalSold = g.Sum(od => od.Quantity),
                    TotalRevenue = g.Sum(od => od.TotalPrice)
                })
                .OrderByDescending(x => x.TotalRevenue)
                .Take(count)
                .ToListAsync();

            return Json(topProducts);
        }

        private (DateTime startDate, DateTime endDate) GetDateRange(string timeRange)
        {
            var endDate = DateTime.Now;
            var startDate = timeRange switch
            {
                "week" => endDate.AddDays(-7),
                "month" => endDate.AddMonths(-1),
                "quarter" => endDate.AddMonths(-3),
                _ => endDate.AddMonths(-1),
            };
            return (startDate, endDate);
        }
    }
}