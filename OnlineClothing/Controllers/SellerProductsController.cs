using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Hosting;
using OnlineClothing.Models;

namespace OnlineClothing.Controllers
{
    public class SellerProductsController : Controller
    {
        private readonly ClothingShopPrn222G2Context _context;
        private readonly IWebHostEnvironment _hostEnvironment;

        //assume the sellerId is 2
        private readonly int sellerId = 2;

        public SellerProductsController(ClothingShopPrn222G2Context context, IWebHostEnvironment hostEnvironment)
        {
            _context = context;
            _hostEnvironment = hostEnvironment;
        }
        public async Task<IActionResult> Index()
        {
            var products = await _context.Products
                .Where(p => p.SellerId.Equals(sellerId))
                .Include(p => p.Category)
                .Include(p => p.StatusNavigation)
                .ToListAsync();
            return View(products);
        }

        public async Task<IActionResult> Add()
        {
            ViewBag.Categories = new SelectList(await _context.Categories.ToListAsync(), "Id", "Name");
            return View("Add");
        }

        [HttpPost]
        public async Task<IActionResult> Add(Product product, IFormFile imageFile)
        {
            if (ModelState.IsValid)
            {
                product.Status = 1;
                product.CreateAt = DateTime.Now;
                //product.SellerId = sellerId;

                if (imageFile != null && imageFile.Length > 0)
                {
                    // Save the image file to a directory
                    string uploadDir = Path.Combine(_hostEnvironment.WebRootPath, "images");
                    string uniqueFileName = Guid.NewGuid().ToString() + "_" + imageFile.FileName;
                    string filePath = Path.Combine(uploadDir, uniqueFileName);
                    using (var fileStream = new FileStream(filePath, FileMode.Create))
                    {
                        await imageFile.CopyToAsync(fileStream);
                    }
                    product.ThumbnailUrl = "/images/" + uniqueFileName;
                }

                _context.Products.Add(product);
                await _context.SaveChangesAsync();
                TempData["message"] = "Added product successfully!";
                return RedirectToAction("Index");
            }
            else
            {
                ViewBag.Categories = new SelectList(await _context.Categories.ToListAsync(), "Id", "Name");
                return View("Add", product);
            }
        }

        public async Task<IActionResult> Details(int id)
        {

            ViewBag.Categories = new SelectList(await _context.Categories.ToListAsync(), "Id", "Name");
            var product = await _context.Products
                .Include(p => p.Category)
                .Include(p => p.StatusNavigation)
                .FirstOrDefaultAsync(p => p.Id == id);
            return View(product);

        }

        [HttpPost]
        public async Task<IActionResult> Update(Product product, IFormFile imageFile)
        {
            var existingProduct = await _context.Products.FirstOrDefaultAsync(p => p.Id == product.Id);
            if (existingProduct != null)
            {
                existingProduct.Name = product.Name;
                existingProduct.Description = product.Description;
                existingProduct.CategoryId = product.CategoryId;
                existingProduct.Price = product.Price;
                existingProduct.Quantity = product.Quantity;
                existingProduct.Status = 2;

                if (imageFile != null && imageFile.Length > 0)
                {
                    // Save the image file to a directory
                    string uploadDir = Path.Combine(_hostEnvironment.WebRootPath, "images");
                    string uniqueFileName = Guid.NewGuid().ToString() + "_" + imageFile.FileName;
                    string filePath = Path.Combine(uploadDir, uniqueFileName);
                    using (var fileStream = new FileStream(filePath, FileMode.Create))
                    {
                        await imageFile.CopyToAsync(fileStream);
                    }
                    existingProduct.ThumbnailUrl = "/images/" + uniqueFileName;
                }

                _context.Update(existingProduct);
                await _context.SaveChangesAsync();
                TempData["message"] = "Updated product successfully!";
                return RedirectToAction("Index");
            }
            else
            {
                return NotFound();
            }
        }

        public async Task<IActionResult> Delete(int id)
        {
            var product = await _context.Products.FirstOrDefaultAsync(p => p.Id == id);
            if (product != null)
            {
                _context.Products.Remove(product);
                await _context.SaveChangesAsync();
                TempData["message"] = "Deleted product successfully!";
                return RedirectToAction("Index");
            }
            else
            {
                return NotFound();
            }
        }
    }
}
