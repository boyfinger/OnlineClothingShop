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

        //assume the sellerId is this?
        private readonly Guid sellerId = new Guid("dde923de-6b2a-4104-a293-6da7aaa68ef3");
        private readonly int pageSize = 4;

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

        private bool IsLoggedIn()
        {
            return HttpContext.Session.GetString("UserId") != null;
        }

        public async Task<IActionResult> Index(string searchString, int categoryId = 0, int page = 1)
        {
            try
            {
                string? userId = HttpContext.Session.GetString("UserId");
                if (userId == null)
                {
                    return RedirectToAction("login", "account");
                }
                var roles = await _context.UserRoles
                    .Where(ur => ur.RoleId == 2 && ur.UserId.Equals(new Guid(userId)))
                    .ToListAsync();
                if (roles == null || roles.Count == 0)
                {
                    ViewData["StatusCode"] = 403;
                    @ViewData["ErrorMessage"] = "You don't have the permission to access this page.";
                    return RedirectToAction("error", "home");
                }

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
                _logger.LogError($"Failed to get products of sellerId = {sellerId}", ex);
                return RedirectToAction("error", "home");
            }
        }

        public async Task<IActionResult> Add()
        {
            try
            {
                string? userId = HttpContext.Session.GetString("UserId");
                if (userId == null)
                {
                    return RedirectToAction("login", "account");
                }
                var roles = await _context.UserRoles
                    .Where(ur => ur.RoleId == 2 && ur.UserId.Equals(new Guid(userId)))
                    .ToListAsync();
                if (roles == null || roles.Count == 0)
                {
                    ViewData["StatusCode"] = 403;
                    @ViewData["ErrorMessage"] = "You don't have the permission to access this page.";
                    return RedirectToAction("error", "home");
                }

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
            try
            {
                string? userId = HttpContext.Session.GetString("UserId");
                if (userId == null)
                {
                    return RedirectToAction("login", "account");
                }
                var roles = await _context.UserRoles
                    .Where(ur => ur.RoleId == 2 && ur.UserId.Equals(new Guid(userId)))
                    .ToListAsync();
                if (roles == null || roles.Count == 0)
                {
                    ViewData["StatusCode"] = 403;
                    @ViewData["ErrorMessage"] = "You don't have the permission to access this page.";
                    return RedirectToAction("error", "home");
                }

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

                if (imageFile == null || imageFile.Length == 0)
                {
                    product.ThumbnailUrl = "/images/default_product.jpg";
                }
                else
                {
                    product.ThumbnailUrl = await _fileUploadService.UploadImageAsync(imageFile);
                }

                _context.Products.Add(product);
                await _context.SaveChangesAsync();

                return RedirectToAction("Index");
            }
            catch (Exception ex)
            {
                _logger.LogError("Failed to add product", ex);
                return RedirectToAction("error", "home");
            }

        }

        public async Task<IActionResult> Details(int id)
        {
            try
            {
                string? userId = HttpContext.Session.GetString("UserId");
                if (userId == null)
                {
                    return RedirectToAction("login", "account");
                }
                var roles = await _context.UserRoles
                    .Where(ur => ur.RoleId == 2 && ur.UserId.Equals(new Guid(userId)))
                    .ToListAsync();
                if (roles == null || roles.Count == 0)
                {
                    ViewData["StatusCode"] = 403;
                    @ViewData["ErrorMessage"] = "You don't have the permission to access this page.";
                    return RedirectToAction("error", "home");
                }

                ViewBag.Categories = new SelectList(await _context.Categories.ToListAsync(), "Id", "Name");
                var product = await _context.Products
                    .Include(p => p.Category)
                    .Include(p => p.StatusNavigation)
                    .FirstOrDefaultAsync(p => p.Id == id);

                if (!product.SellerId.Equals(new Guid(userId)))
                {
                    ViewData["StatusCode"] = 403;
                    @ViewData["ErrorMessage"] = "You don't have the permission to access this page.";
                    return RedirectToAction("error", "home");
                }

                return View(product);
            }
            catch (Exception ex)
            {
                _logger.LogError($"Failed to get details for productId = {id}", ex);
                return RedirectToAction("error", "home");
            }
        }

        [HttpPost]
        public async Task<IActionResult> Update(Product product)
        {
            try
            {
                string? userId = HttpContext.Session.GetString("UserId");
                if (userId == null)
                {
                    return RedirectToAction("login", "account");
                }
                var roles = await _context.UserRoles
                    .Where(ur => ur.RoleId == 2 && ur.UserId.Equals(new Guid(userId)))
                    .ToListAsync();
                if (roles == null || roles.Count == 0)
                {
                    ViewData["StatusCode"] = 403;
                    @ViewData["ErrorMessage"] = "You don't have the permission to access this page.";
                    return RedirectToAction("error", "home");
                }

                if (!ModelState.IsValid)
                {
                    ViewBag.Categories = new SelectList(await _context.Categories.ToListAsync(), "Id", "Name");
                    return View("Details", product);
                }

                var existingProduct = await _context.Products.FirstOrDefaultAsync(p => p.Id == product.Id);
                if (existingProduct == null)
                {
                    _logger.LogWarning($"Product with id = {product.Id} not found");
                    return NotFound();
                }

                if (!existingProduct.SellerId.Equals(new Guid(userId)))
                {
                    ViewData["StatusCode"] = 403;
                    @ViewData["ErrorMessage"] = "You don't have the permission to access this page.";
                    return RedirectToAction("error", "home");
                }

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
                return RedirectToAction("error", "home");
            }
        }

        [HttpPost]
        public async Task<IActionResult> UpdateDiscount(int id, int discount)
        {
            try
            {
                string? userId = HttpContext.Session.GetString("UserId");
                if (userId == null)
                {
                    return RedirectToAction("login", "account");
                }
                var roles = await _context.UserRoles
                    .Where(ur => ur.RoleId == 2 && ur.UserId.Equals(new Guid(userId)))
                    .ToListAsync();
                if (roles == null || roles.Count == 0)
                {
                    ViewData["StatusCode"] = 403;
                    @ViewData["ErrorMessage"] = "You don't have the permission to access this page.";
                    return RedirectToAction("error", "home");
                }

                var product = await _context.Products.FirstOrDefaultAsync(p => p.Id == id);
                if (product == null)
                {
                    _logger.LogWarning($"Product with id = {id} not found");
                    return NotFound();
                }

                if (!product.SellerId.Equals(new Guid(userId)))
                {
                    ViewData["StatusCode"] = 403;
                    @ViewData["ErrorMessage"] = "You don't have the permission to access this page.";
                    return RedirectToAction("error", "home");
                }

                product.Discount = discount;

                _context.Products.Update(product);
                await _context.SaveChangesAsync();

                return Ok();
            }
            catch (Exception ex)
            {
                _logger.LogError($"Failed to deactivate product with id = {id}", ex);
                return RedirectToAction("error", "home");
            }
        }

        [HttpPost]
        public async Task<IActionResult> SetThumbnail(int id, IFormFile? imageFile)
        {
            try
            {
                string? userId = HttpContext.Session.GetString("UserId");
                if (userId == null)
                {
                    return RedirectToAction("login", "account");
                }
                var roles = await _context.UserRoles
                    .Where(ur => ur.RoleId == 2 && ur.UserId.Equals(new Guid(userId)))
                    .ToListAsync();
                if (roles == null || roles.Count == 0)
                {
                    ViewData["StatusCode"] = 403;
                    @ViewData["ErrorMessage"] = "You don't have the permission to access this page.";
                    return RedirectToAction("error", "home");
                }

                var url = await _fileUploadService.UploadImageAsync(imageFile);
                var existingProduct = await _context.Products.FirstOrDefaultAsync(p => p.Id == id);
                if (existingProduct == null)
                {
                    _logger.LogWarning($"Product with id = {id} not found");
                    return NotFound();
                }

                if (!existingProduct.SellerId.Equals(new Guid(userId)))
                {
                    ViewData["StatusCode"] = 403;
                    @ViewData["ErrorMessage"] = "You don't have the permission to access this page.";
                    return RedirectToAction("error", "home");
                }

                existingProduct.ThumbnailUrl = url;
                existingProduct.Status = 2;

                _context.Products.Update(existingProduct);
                await _context.SaveChangesAsync();

                return Ok();
            }
            catch (Exception ex)
            {
                _logger.LogError($"Failed to set thumbnail for product with id = {id}", ex);
                return RedirectToAction("error", "home");
            }
        }
    }
}
