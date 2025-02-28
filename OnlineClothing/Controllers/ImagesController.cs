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
        public async Task<IActionResult> GetAllProductImages(int productId)
        {
            try
            {
                ViewBag.Product = await _context.Products.FirstOrDefaultAsync(p => p.Id == productId);
                var imageList = await _context.Images
                    .Where(i => i.ProductId == productId)
                    .Include(i => i.Product)
                    .ToListAsync();
                return View("~/Views/SellerProducts/ProductImages.cshtml", imageList);
            }
            catch (Exception ex)
            {
                _logger.LogError($"Failed to get images of productId = {productId}", ex);
                return RedirectToAction("error", "home");
            }
        }

        [HttpPost]
        [Route("/sellerproducts/{productId}/images/add")]
        public async Task<IActionResult> AddProductImage(int productId, IFormFile? imageFile)
        {
            try
            {
                var imageUrl = await _fileUploadService.UploadImageAsync(imageFile);
                var image = new Image
                {
                    ProductId = productId,
                    Url = imageUrl
                };
                _context.Images.Add(image);
                await _context.SaveChangesAsync();
                return RedirectToAction("getallproductimages", new { productId });
            }
            catch (Exception ex)
            {
                _logger.LogError($"Failed to add image for product with id = {productId}", ex);
                return View("error", "home");
            }
        }

        [HttpPost]
        [Route("sellerproducts/{productId}/image/delete/{id}")]
        public async Task<IActionResult> DeleteProductImage(int id, int productId)
        {
            try
            {
                var image = await _context.Images.FirstOrDefaultAsync(i => i.Id == id);
                if (image == null)
                {
                    _logger.LogWarning($"Image with id = {id} not found");
                    return NotFound();
                }
                    _context.Images.Remove(image);
                    await _context.SaveChangesAsync();
                
                return RedirectToAction("getallproductimages", new { productId });
            }
            catch (Exception ex)
            {
                _logger.LogError($"Failed to add image with id = {id}", ex);
                return View("error", "home");
            }
        }
    }
}
