using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
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
        private readonly IOpenAIService _openAIService;

        private readonly int pageSize = 4;

        public SellerProductsController(
            ClothingShopPrn222G2Context context,
            IWebHostEnvironment hostEnvironment,
            IFileUploadService fileUploadService,
            ILogger<SellerProductsController> logger,
            IOpenAIService openAIService)
        {
            _context = context;
            _hostEnvironment = hostEnvironment;
            _fileUploadService = fileUploadService;
            _logger = logger;
            _openAIService = openAIService;
        }

        public async Task<IActionResult> Index(string searchString, int categoryId = 0, int page = 1)
        {
            try
            {
                var userId = HttpContext.Session.GetString("UserId");
                if (userId == null)
                {
                    return RedirectToAction("Login", "Account");
                }

                var userRole = HttpContext.Session.GetString("UserRole");
                if (userRole != "SELLER")
                {
                    return RedirectToAction("handleerror", "error", new { statusCode = 403 });
                }

                var sellerId = new Guid(userId);
                var query = _context.Products
                    .Where(p => p.SellerId.Equals(sellerId))
                    .Include(p => p.Category)
                    .Include(p => p.StatusNavigation)
                    .AsQueryable();

                if (!string.IsNullOrEmpty(searchString))
                {
                    query = query.Where(p => p.Name.Contains(searchString));
                }

                if (categoryId != 0)
                {
                    query = query.Where(p => p.CategoryId == categoryId);
                }

                var totalProducts = await query.CountAsync();
                var totalPages = (int)Math.Ceiling(totalProducts / (double)pageSize);

                var products = await query
                    .OrderBy(p => p.Name)
                    .Skip((page - 1) * pageSize)
                    .Take(pageSize)
                    .ToListAsync();

                ViewBag.CurrentPage = page;
                ViewBag.TotalPages = totalPages;
                ViewBag.SearchString = searchString;
                ViewBag.SelectedCategoryId = categoryId;
                ViewBag.Categories = new SelectList(await _context.Categories.ToListAsync(), "Id", "Name");

                return View(products);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error while getting products for seller");
                return RedirectToAction("handleerror", "error", new { statusCode = 500 });
            }
        }

        public async Task<IActionResult> Add()
        {
            try
            {
                var userId = HttpContext.Session.GetString("UserId");
                if (userId == null)
                {
                    return RedirectToAction("Login", "Account");
                }

                var userRole = HttpContext.Session.GetString("UserRole");
                if (userRole != "SELLER")
                {
                    return RedirectToAction("handleerror", "error", new { statusCode = 403 });
                }

                ViewBag.Categories = new SelectList(await _context.Categories.ToListAsync(), "Id", "Name");
                return View("Add");
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Failed to get category list to display for ProductAdd page");
                return RedirectToAction("handleerror", "error", new { statusCode = 500 });
            }
        }

        [HttpPost]
        public async Task<IActionResult> Add(Product product, IFormFile? imageFile)
        {
            try
            {
                var userId = HttpContext.Session.GetString("UserId");
                if (userId == null)
                {
                    return RedirectToAction("Login", "Account");
                }

                var userRole = HttpContext.Session.GetString("UserRole");
                if (userRole != "SELLER")
                {
                    return RedirectToAction("handleerror", "error", new { statusCode = 403 });
                }

                var sellerId = new Guid(userId);

                if (!ModelState.IsValid)
                {
                    ViewBag.Categories = new SelectList(await _context.Categories.ToListAsync(), "Id", "Name");

                    return View("Add", product);
                }

                product.Status = 2;
                product.CreateAt = DateTime.Now;
                product.SellerId = sellerId;

                if (product.Discount == null)
                {
                    product.Discount = 0;
                }

                // changed by tuan anh (only add image validating + condition)
                string openAIResponse = null;
                if (imageFile == null || imageFile.Length == 0)
                {
                    product.ThumbnailUrl = "/images/default_product.jpg";
                }
                else
                {
                    // main changes
                    string description = product.Description == null? "" : product.Description;
                    openAIResponse = await _openAIService.CheckImage(imageFile, description);
                    openAIResponse ??= "Cannot connect to OpenAI for validating image";
                    if (!openAIResponse.Equals("Valid"))
                    {
                        TempData["openAIResponse"] = openAIResponse;
                        return RedirectToAction("Add");
                    }
                    // end of changes

                    product.ThumbnailUrl = await _fileUploadService.UploadImageAsync(imageFile);
                }

                _context.Products.Add(product);
                await _context.SaveChangesAsync();

                return RedirectToAction("Index");
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Failed to add product");
                return RedirectToAction("handleerror", "error", new { statusCode = 500 });
            }

        }

        public async Task<IActionResult> Details(int id)
        {
            try
            {
                var userId = HttpContext.Session.GetString("UserId");
                if (userId == null)
                {
                    return RedirectToAction("Login", "Account");
                }

                var sellerId = new Guid(userId);
                var userRole = HttpContext.Session.GetString("UserRole");

                var product = await _context.Products
                    .Include(p => p.Category)
                    .Include(p => p.StatusNavigation)
                    .FirstOrDefaultAsync(p => p.Id == id && p.SellerId.Equals(sellerId));

                if (userRole != "SELLER" || product == null)
                {
                    return RedirectToAction("handleerror", "error", new { statusCode = 403 });
                }

                ViewBag.Categories = new SelectList(await _context.Categories.ToListAsync(), "Id", "Name");
                return View(product);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, $"Failed to get details for productId = {id}");
                return RedirectToAction("handleerror", "error", new { statusCode = 500 });
            }
        }

        [HttpPost]
        public async Task<IActionResult> Update(Product product)
        {
            try
            {
                var userId = HttpContext.Session.GetString("UserId");
                if (userId == null)
                {
                    return RedirectToAction("Login", "Account");
                }

                var sellerId = new Guid(userId);
                var userRole = HttpContext.Session.GetString("UserRole");

                var existingProduct = await _context.Products.FirstOrDefaultAsync(p => p.Id == product.Id && p.SellerId.Equals(sellerId));

                if (userRole != "SELLER" || product == null)
                {
                    return RedirectToAction("handleerror", "error", new { statusCode = 403 });
                }

                if (!ModelState.IsValid)
                {
                    ViewBag.Categories = new SelectList(await _context.Categories.ToListAsync(), "Id", "Name");
                    return View("Details", product);
                }

                
                // openAI validate description
                string description = product.Description == null ? "" : product.Description;
                string openAIResponse = await _openAIService.CheckDescriptionAsync(description);
                if (openAIResponse == null)
                {
                    openAIResponse = "Cannot connect to OpenAI to validate description";
                }
                if (!openAIResponse.Equals("Valid"))
                {
                    TempData["openAIResponse"] = openAIResponse;
                    return RedirectToAction("Add");
                }
                // end of changes


                existingProduct.Name = product.Name;
                existingProduct.Description = product.Description;
                existingProduct.CategoryId = product.CategoryId;
                existingProduct.Price = product.Price;
                existingProduct.Quantity = product.Quantity;
                existingProduct.Status = 2;     //to unapproved
                existingProduct.UpdateAt = DateTime.Now;

                _context.Update(existingProduct);
                await _context.SaveChangesAsync();

                // Store a success message in TempData
                TempData["SuccessMessage"] = "Product updated successfully!";

                // Redirect to the index or another desired page
                return RedirectToAction("index");
            }
            catch (Exception ex)
            {
                _logger.LogError($"Failed to update product with id = {product.Id}", ex);
                return RedirectToAction("handleerror", "error", new { statusCode = 500 });
            }
        }

        [HttpPost]
        public async Task<IActionResult> UpdateDiscount(int id, int discount)
        {
            try
            {
                var userId = HttpContext.Session.GetString("UserId");
                if (userId == null)
                {
                    return RedirectToAction("Login", "Account");
                }

                var sellerId = new Guid(userId);
                var userRole = HttpContext.Session.GetString("UserRole");

                var product = await _context.Products.FirstOrDefaultAsync(p => p.Id == id && p.SellerId.Equals(sellerId));

                if (userRole != "SELLER" || product == null)
                {
                    return RedirectToAction("handleerror", "error", new { statusCode = 403 });
                }

                product.Discount = discount;

                _context.Products.Update(product);
                await _context.SaveChangesAsync();

                return Ok();
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, $"Failed to deactivate product with id = {id}");
                return RedirectToAction("handleerror", "error", new { statusCode = 500 });
            }
        }

        [HttpPost]
        public async Task<IActionResult> SetThumbnail(int id, IFormFile? imageFile)
        {
            try
            {
                var userId = HttpContext.Session.GetString("UserId");
                if (userId == null)
                {
                    return RedirectToAction("Login", "Account");
                }

                var sellerId = new Guid(userId);
                var userRole = HttpContext.Session.GetString("UserRole");

                var existingProduct = await _context.Products.FirstOrDefaultAsync(p => p.Id == id && p.SellerId.Equals(sellerId));

                if (userRole != "SELLER" || existingProduct == null)
                {
                    return RedirectToAction("handleerror", "error", new { statusCode = 403 });
                }

                var url = await _fileUploadService.UploadImageAsync(imageFile);

                existingProduct.ThumbnailUrl = url;
                existingProduct.Status = 2;

                _context.Products.Update(existingProduct);
                await _context.SaveChangesAsync();

                return Ok();
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, $"Failed to set thumbnail for product with id = {id}");
                return RedirectToAction("handleerror", "error", new { statusCode = 500 });
            }
        }
    }
}
