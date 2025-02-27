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

        public ImagesController(ClothingShopPrn222G2Context context, IFileUploadService fileUploadService)
        {
            _context = context;
            _fileUploadService = fileUploadService;
        }

        [Route("/sellerproducts/{productId}/images")]
        public async Task<IActionResult> GetAllProductImages(int productId)
        {
            ViewBag.Product = await _context.Products.FirstOrDefaultAsync(p => p.Id == productId);
            var imageList = await _context.Images
                .Where(i => i.ProductId == productId)
                .Include(i => i.Product)
                .ToListAsync();
            return View("~/Views/SellerProducts/ProductImages.cshtml", imageList);
        }

        [HttpPost]
        [Route("/sellerproducts/{productId}/images/add")]
        public async Task<IActionResult> AddProductImage(int productId, IFormFile? imageFile)
        {
            var imageUrl = await _fileUploadService.UploadImageAsync(imageFile);
            var image = new Image
            {
                ProductId = productId,
                Url = imageUrl
            };
            _context.Images.Add(image);
            await _context.SaveChangesAsync();
            return RedirectToAction("getallproductimages", new {productId});
        }

        [HttpPost]
        [Route("sellerproducts/{productId}/image/delete/{id}")]
        public async Task<IActionResult> DeleteProductImage(int id, int productId)
        {
            var image = await _context.Images.FirstOrDefaultAsync(i => i.Id == id);
            if (image != null)
            {
                _context.Images.Remove(image);
                await _context.SaveChangesAsync();
            }
            return RedirectToAction("getallproductimages", new { productId });
        }
    }
}
