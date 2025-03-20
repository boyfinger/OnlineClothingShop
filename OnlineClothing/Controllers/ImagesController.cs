using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using OnlineClothing.Models;
using OnlineClothing.Services;

namespace OnlineClothing.Controllers
{
    public class ImagesController : Controller
    {
        private readonly ClothingShopPrn222G2Context _context;
        private readonly IFileUploadService _fileUploadService;
        private readonly ILogger<ImagesController> _logger;

        private readonly int pageSize = 4;

        public ImagesController(
            ClothingShopPrn222G2Context context,
            IFileUploadService fileUploadService,
            ILogger<ImagesController> logger)
        {
            _context = context;
            _fileUploadService = fileUploadService;
            _logger = logger;
        }

        [Route("/sellerproducts/{productId}/images")]
        public async Task<IActionResult> GetAllProductImages(int productId, int page = 1)
        {
            try
            {
                string? userId = HttpContext.Session.GetString("UserId");
                if (userId == null)
                {
                    return RedirectToAction("login", "account");
                }
                var userRole = HttpContext.Session.GetString("UserRole");
                if (userRole != "SELLER")
                {
                    return RedirectToAction("handleerror", "error", new { statusCode = 403 });
                }

                var product = await _context.Products.FirstOrDefaultAsync(p => p.Id == productId);
                if (product == null)
                {
                    _logger.LogWarning($"Product with id = {productId} not found");
                    return NotFound();
                }

                if (!product.SellerId.Equals(new Guid(userId)))
                {
                    return RedirectToAction("handleerror", "error", new { statusCode = 403 });
                }

                ViewBag.Product = product;
                var query = _context.Images
                    .Where(i => i.ProductId == productId)
                    .Include(i => i.Product)
                    .AsQueryable();

                var totalProducts = await query.CountAsync();
                var totalPages = (int)Math.Ceiling(totalProducts / (double)pageSize);


                var imageList = await query
                    .Skip((page - 1) * pageSize)
                    .Take(pageSize)
                    .ToListAsync();
                ViewBag.ProductId = productId;
                ViewBag.CurrentPage = page;
                ViewBag.TotalPages = totalPages;
                return View("~/Views/SellerProducts/ProductImages.cshtml", imageList);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, $"Failed to get images of productId = {productId}");
                return RedirectToAction("handleerror", "error", new { statusCode = 500 });
            }
        }

        [HttpPost]
        [Route("/sellerproducts/{productId}/images/add")]
        public async Task<IActionResult> AddProductImage(int productId, IFormFile? imageFile)
        {
            try
            {
                string? userId = HttpContext.Session.GetString("UserId");
                if (userId == null)
                {
                    return RedirectToAction("login", "account");
                }
                var userRole = HttpContext.Session.GetString("UserRole");
                if (userRole != "SELLER")
                {
                    return RedirectToAction("handleerror", "error", new { statusCode = 403 });
                }

                var product = await _context.Products.FirstOrDefaultAsync(p => p.Id == productId);
                if (product == null)
                {
                    _logger.LogWarning($"Product with id = {productId} not found");
                    return NotFound();
                }

                if (!product.SellerId.Equals(new Guid(userId)))
                {
                    return RedirectToAction("handleerror", "error", new { statusCode = 403 });
                }

                var imageUrl = await _fileUploadService.UploadImageAsync(imageFile);
                var image = new Image
                {
                    ProductId = productId,
                    Url = imageUrl
                };
                _context.Images.Add(image);
                await _context.SaveChangesAsync();

                return Ok();
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, $"Failed to add image for product with id = {productId}");
                return RedirectToAction("handleerror", "error", new { statusCode = 500 });
            }
        }

        [HttpPost]
        [Route("sellerproducts/image/delete/{id}")]
        public async Task<IActionResult> DeleteProductImage(int id)
        {
            try
            {
                string? userId = HttpContext.Session.GetString("UserId");
                if (userId == null)
                {
                    return RedirectToAction("login", "account");
                }
                var userRole = HttpContext.Session.GetString("UserRole");
                if (userRole != "SELLER")
                {
                    return RedirectToAction("handleerror", "error", new { statusCode = 403 });
                }

                var image = await _context.Images.FirstOrDefaultAsync(i => i.Id == id);
                if (image == null)
                {
                    _logger.LogWarning($"Image with id = {id} not found");
                    return NotFound();
                }

                var product = await _context.Products.FirstOrDefaultAsync(p => p.Id == image.ProductId);
                if (product == null)
                {
                    _logger.LogWarning($"Product with id = {image.ProductId} not found");
                    return NotFound();
                }

                if (!product.SellerId.Equals(new Guid(userId)))
                {
                    return RedirectToAction("handleerror", "error", new { statusCode = 403 });
                }

                _context.Images.Remove(image);
                await _context.SaveChangesAsync();

                return Ok();
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, $"Failed to add image with id = {id}");
                return RedirectToAction("handleerror", "error", new { statusCode = 500 });
            }
        }
    }
}
