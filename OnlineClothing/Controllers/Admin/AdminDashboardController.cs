using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using OnlineClothing.Models;

namespace OnlineClothing.Controllers.AdminController
{
    public class AdminDashboardController : Controller
    {
        private readonly ClothingShopPrn222G2Context _context;
        public AdminDashboardController()
        {
            _context = new ClothingShopPrn222G2Context();
        }

        [HttpGet]
        public async Task<IActionResult> DashboardAsync()
        {
            var userRole = HttpContext.Session.GetString("UserRole");

            if (string.IsNullOrEmpty(userRole) || userRole != "ADMIN")
            {
                return RedirectToAction("AdminLogin", "Account"); 
            }
            var totalUsers = await _context.Users.CountAsync();
            var totalProducts = await _context.Products.CountAsync();
            var totalShops = await _context.Users
                .Include(u => u.UserRoles)
                .CountAsync(u => u.UserRoles.Any(ur => ur.RoleId == 2));
            ViewBag.TotalUsers = totalUsers;
            ViewBag.TotalProducts = totalProducts;
            ViewBag.TotalShops = totalShops;
            return View();
        }
    }
}
