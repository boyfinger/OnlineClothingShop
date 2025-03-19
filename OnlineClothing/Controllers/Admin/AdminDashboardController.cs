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
        public IActionResult Dashboard()
        {
            var userRole = HttpContext.Session.GetString("UserRole");

            if (string.IsNullOrEmpty(userRole) || userRole != "ADMIN")
            {
                return RedirectToAction("AdminLogin", "Account"); 
            }
            var totalUsers = _context.Users.Count();
            ViewBag.TotalUsers = totalUsers;
            return View();
        }
    }
}
