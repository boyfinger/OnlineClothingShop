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
        private readonly ILogger<SellerProductsController> _logger;

        //assume the sellerId is this?
        private readonly Guid sellerId = new Guid("dde923de-6b2a-4104-a293-6da7aaa68ef3");

        public SellerProductsController(
            ClothingShopPrn222G2Context context,
            IWebHostEnvironment hostEnvironment,
            IFileUploadService fileUploadService,
            ILogger<SellerProductsController> logger)
        {
            _context = context;
            _hostEnvironment = hostEnvironment;
            _fileUploadService = fileUploadService;
            _logger = logger;
        }
        public async Task<IActionResult> Index()
        {
            try
            {
                var products = await _context.Products
                    .Where(p => p.SellerId.Equals(sellerId))
                    .Include(p => p.Category)
                    .Include(p => p.StatusNavigation)
                    .ToListAsync();
                return View(products);
            }
            catch (Exception ex)
            {
                _logger.LogError($"Failed to get products of sellerId = {sellerId}", ex);
                return RedirectToAction("error", "home");
            }
        }

        public async Task<IActionResult> Add()
        {
            try
            {
                ViewBag.Categories = new SelectList(await _context.Categories.ToListAsync(), "Id", "Name");
                return View("Add");
            }
            catch (Exception ex)
            {
                _logger.LogError("Failed to get category list to display for ProductAdd page", ex);
                return RedirectToAction("error", "home");
            }
        }

        [HttpPost]
        public async Task<IActionResult> Add(Product product, IFormFile? imageFile)
        {
            if (ModelState.IsValid)
            {
                try
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
                catch (Exception ex)
                {
                    _logger.LogError("Failed to add product", ex);
                    return RedirectToAction("error", "home");
                }
            }
            else
            {
                ViewBag.Categories = new SelectList(await _context.Categories.ToListAsync(), "Id", "Name");
                return View("Add", product);
            }
        }

        public async Task<IActionResult> Details(int id)
        {
            try
            {
                ViewBag.Categories = new SelectList(await _context.Categories.ToListAsync(), "Id", "Name");
                var product = await _context.Products
                    .Include(p => p.Category)
                    .Include(p => p.StatusNavigation)
                    .FirstOrDefaultAsync(p => p.Id == id);
                return View(product);
            }
            catch (Exception ex)
            {
                _logger.LogError($"Failed to get details for productId = {id}", ex);
                return RedirectToAction("error", "home");
            }
        }

        [HttpPost]
        public async Task<IActionResult> Update(Product product, IFormFile? imageFile)
        {
            try
            {
                var existingProduct = await _context.Products.FirstOrDefaultAsync(p => p.Id == product.Id);
                if (existingProduct == null)
                {
                    _logger.LogWarning($"Product with id = {product.Id} not found");
                    return NotFound();
                }

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
                catch { }

                _context.Update(existingProduct);
                await _context.SaveChangesAsync();
                TempData["message"] = "Updated product successfully!";
                return RedirectToAction("Index");
            }
            catch (Exception ex)
            {

                _logger.LogError($"Failed to update product with id = {product.Id}", ex);
                return RedirectToAction("error", "home");
            }
        }

        public async Task<IActionResult> Deactivate(int id)
        {
            try
            {
                var product = await _context.Products.FirstOrDefaultAsync(p => p.Id == id);
                if (product == null)
                {
                    _logger.LogWarning($"Product with id = {id} not found");
                    return NotFound();
                }

                //set product to Discontinued
                product.Status = 3;
                _context.Update(product);
                await _context.SaveChangesAsync();
                TempData["message"] = "Deactivate product successfully!";
                return RedirectToAction("Index");
            }
            catch (Exception ex)
            {
                _logger.LogError($"Failed to deactivate product with id = {id}", ex);
                return RedirectToAction("error", "home");
            }
        }
    }
}
