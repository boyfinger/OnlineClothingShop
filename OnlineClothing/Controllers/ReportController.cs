using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration.UserSecrets;
using OnlineClothing.Models;

namespace OnlineClothing.Controllers
{
    public class ReportController : Controller
    {
        private readonly ClothingShopPrn222G2Context _context;

        public ReportController(ClothingShopPrn222G2Context context)
        {
            _context = context;
        }


        public async Task<IActionResult> Create(long productId)
        {
            var userId = HttpContext.Session.GetString("UserId");
            if (string.IsNullOrEmpty(userId))
            {
                return RedirectToAction("Login", "Account");
            }
            var product = await _context.Products.Include(p => p.Category).FirstOrDefaultAsync(p => p.Id == productId);
            if (product == null)
            {
                return NotFound();
            }
            var report = new Report
            {
                Product = product,
                ProductId = productId,
                CreateAt = DateTime.Now,
                Status = 1
            };

            return View(report);
        }

        [HttpPost]
        public async Task<IActionResult> Create(Report report)
        {
            var userId = HttpContext.Session.GetString("UserId");
            if (string.IsNullOrEmpty(userId))
            {
                return RedirectToAction("Login", "Account");
            }
            if (string.IsNullOrEmpty(report.Reason) || report.Reason.Length < 10)
            {
                TempData["ErrorMessage"] = "Reason must be at least 10 characters long.";
                report.Product = await _context.Products.Include(p => p.Category)
                            .FirstOrDefaultAsync(p => p.Id == report.ProductId);

                return View(report);
            }
            report.UserId = Guid.Parse(userId);
            report.CreateAt = DateTime.Now;
            report.Status = 1;

            await _context.Reports.AddAsync(report);
            _context.SaveChanges();

            TempData["SuccessMessage"] = "Report submitted successfully!";
            return RedirectToAction("Success");
        }

        public IActionResult Success()
        {
            return View();
        }
    }
}
