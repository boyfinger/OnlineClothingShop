using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Hosting;
using OnlineClothing.Models;
using OnlineClothing.Services;

namespace OnlineClothing.Controllers
{
    public class SellerProductsController : Controller
    {
        private readonly ClothingShopPrn222G2Context _context;
        private readonly IWebHostEnvironment _hostEnvironment;
        private readonly IFileUploadService _fileUploadService;

        //assume the sellerId is this?
        private readonly Guid sellerId = new Guid("dde923de-6b2a-4104-a293-6da7aaa68ef3");

        public SellerProductsController(
            ClothingShopPrn222G2Context context, 
            IWebHostEnvironment hostEnvironment,
            IFileUploadService fileUploadService)
        {
            _context = context;
            _hostEnvironment = hostEnvironment;
            _fileUploadService = fileUploadService;
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
        public async Task<IActionResult> Add(Product product, IFormFile? imageFile)
        {
            if (ModelState.IsValid)
            {
                product.Status = 1;
                product.CreateAt = DateTime.Now;
                product.SellerId = sellerId;
                try
                {
                    product.ThumbnailUrl = await _fileUploadService.UploadImageAsync(imageFile);
                }
                catch
                {
                    product.ThumbnailUrl = null;
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
        public async Task<IActionResult> Update(Product product, IFormFile? imageFile)
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

                try
                {
                    existingProduct.ThumbnailUrl = await _fileUploadService.UploadImageAsync(imageFile);
                }
                catch
                {

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

        public async Task<IActionResult> Deactivate(int id)
        {
            var product = await _context.Products.FirstOrDefaultAsync(p => p.Id == id);
            if (product != null)
            {
                //set product to Discontinued
                product.Status = 3;
                _context.Update(product);
                await _context.SaveChangesAsync();
                TempData["message"] = "Deactivate product successfully!";
                return RedirectToAction("Index");
            }
            else
            {
                return NotFound();
            }
        }
    }
}
