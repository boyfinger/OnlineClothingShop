using System.Diagnostics;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using OnlineClothing.Models;

namespace OnlineClothing.Controllers
{
    public class HomeController : Controller
    {
        private readonly ILogger<HomeController> _logger;
        private readonly ClothingShopPrn222G2Context _context;

        public HomeController(ILogger<HomeController> logger, ClothingShopPrn222G2Context context)
        {
            _logger = logger;
            _context = context;
        }
        public async Task<IActionResult> Index()
        {
            int totalProducts = await _context.Products.CountAsync();
            int skip = new Random().Next(0, Math.Max(1, totalProducts - 4));
            List<Product> hotproducts = await _context.Products
                .Skip(skip)
                .Take(5)
                .ToListAsync();
            List<Product> saleProducts = await _context.Products
                .Where(p => p.Discount > 0)
                .OrderByDescending(p => p.Discount)
                .Take(5)
                .ToListAsync();
            var homeModel = new HomeViewModel
            {
                hotProducts = hotproducts,
                saleProducts = saleProducts
            };
            return View(homeModel);
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
