using System.Diagnostics;
using Microsoft.AspNetCore.Mvc;
using OnlineClothing.Models;

namespace OnlineClothing.Controllers
{
    public class HomeController : Controller
    {
        private readonly ILogger<HomeController> _logger;

        public HomeController(ILogger<HomeController> logger)
        {
            _logger = logger;
        }
            
		public IActionResult Index()
		{
            ViewBag.UserId = HttpContext.Session.GetString("UserId");
            if(ViewBag.UserId != null)
            {
                ViewBag.UserRole = HttpContext.Session.GetString("UserRole");
                ViewBag.AvatarUrl = HttpContext.Session.GetString("AvatarUrl") ?? "/images/default-avatar.jpg";
            }
			return View();
		}


		public IActionResult Privacy()
        {
            return View();
        }

        [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
        public IActionResult Error()
        {
            return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
        }
    }
}
