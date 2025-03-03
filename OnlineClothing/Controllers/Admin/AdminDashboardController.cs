using Microsoft.AspNetCore.Mvc;

namespace OnlineClothing.Controllers.AdminController
{
    public class AdminDashboardController : Controller
    {
        [HttpGet]
        public IActionResult Dashboard()
        {
            var userRole = HttpContext.Session.GetString("UserRole");

            if (string.IsNullOrEmpty(userRole) || userRole != "ADMIN")
            {
                return RedirectToAction("AdminLogin", "Account"); 
            }

            return View();
        }
    }
}
