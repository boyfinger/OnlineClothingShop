using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Caching.Memory;
using OnlineClothing.Models;

namespace OnlineClothing.Controllers
{
    public class ProductController : Controller
    {

        private readonly ClothingShopPrn222G2Context _context;
        private readonly IMemoryCache _cache;

        public ProductController(ClothingShopPrn222G2Context context, IMemoryCache cache)
        {
            _context = context;
            _cache = cache;
        }

        /* 
          Retrieves the list of product categories from cache or database if not cached.
          Returns: List of product categories.
        */
        private async Task<List<Category>> GetCategories()
        {
            if (!_cache.TryGetValue("categories", out List<Category>? categories))
            {
                categories = await _context.Categories.ToListAsync();
                _cache.Set("categories", categories, TimeSpan.FromMinutes(10));
            }
            return categories!;
        }

        /*
          Retrieves the list of product statuses from cache or database if not cached.
          Returns: List of product statuses.
        */
        private async Task<List<ProductStatus>> GetProductStatuses()
        {
            if (!_cache.TryGetValue("statuses", out List<ProductStatus>? statuses))
            {
                statuses = await _context.ProductStatuses.ToListAsync();
                _cache.Set("statuses", statuses, TimeSpan.FromMinutes(10));
            }
            return statuses!;
        }

        public async Task<IActionResult> Index(int page = 1, int pageSize = 8)
        {
            try
            {
                int totalProducts = await _context.Products.CountAsync();
                List<Product> products = await _context.Products
                    .OrderBy(p => p.Id)
                    .Skip((page - 1) * pageSize)
                    .Take(pageSize)
                    .ToListAsync();
                var viewModel = new ProductViewModel
                {
                    Products = products,
                    Categories = await GetCategories(),
                    ProductStatuses = await GetProductStatuses(),
                    CurrentPage = page,
                    TotalPages = (int)Math.Ceiling(totalProducts / (double)pageSize)
                };
                return View("Product", viewModel);
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Exception when getting products: {ex.Message}");
                return View("Error");
            }
        }

        public async Task<ActionResult<IEnumerable<Product>>> Detail(int id)
        {
            var product = await _context.Products
                .Include(p => p.Images)
                .Include(p => p.Category)
                .Include(p => p.Seller)
                    .ThenInclude(p => p.Userinfo)
                .Include(p => p.Feedbacks)
                    .ThenInclude(f => f.User)
                .FirstOrDefaultAsync(p => p.Id == id);
            if (product == null)
            {
                return NotFound();
            }
            var relatedProducts = await _context.Products
                .Where(p => p.CategoryId == product.CategoryId || p.SellerId == product.SellerId && p.Id != product.Id)
                .Select(p => new Product
                {
                    Id = p.Id,
                    Name = p.Name,
                    Description = p.Description,
                    Price = p.Price,
                    ThumbnailUrl = p.ThumbnailUrl,
                    Discount = p.Discount,
                })
                .Take(5)
                .ToListAsync();

            var viewModel = new ProductDetailViewModel
            {
                Product = product,
                RelatedProducts = relatedProducts,
            };

            return View("Detail", viewModel);
        }

        /*
          Searches for products based on a given query.
          The search is performed on both the product name and description.
          Parameters: query - The search keyword entered by the user.
          Returns: A view displaying the list of matching products along with categories and statuses.
        */
        public async Task<IActionResult> Search(string query, int page = 1, int pageSize = 8)
        {
            try
            {
                if (string.IsNullOrWhiteSpace(query))
                {
                    return RedirectToAction("Index");
                }
                var queryable = _context.Products
                    .AsNoTracking()
                    .Where(p => p.Status == 1 &&
                                (p.Name.ToLower().Contains(query.ToLower()) ||
                                 p.Description.ToLower().Contains(query.ToLower())));
                int totalProducts = await queryable.CountAsync();
                List<Product> products = await queryable
                    .OrderBy(p => p.Id)
                    .Skip((page - 1) * pageSize)
                    .Take(pageSize)
                    .ToListAsync();
                var viewModel = new ProductViewModel
                {
                    Products = products,
                    Categories = await GetCategories(),
                    ProductStatuses = await GetProductStatuses(),
                    CurrentPage = page,
                    TotalPages = (int)Math.Ceiling(totalProducts / (double)pageSize),
                    SearchQuery = query
                };

                return View("Product", viewModel);
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Exception when searching products: {ex.Message}");
                return View("Error");
            }
        }

        [HttpGet]
        public async Task<IActionResult> SearchSuggestions(string query)
        {
            if (string.IsNullOrWhiteSpace(query))
                return Json(new List<object>());
            var products = await _context.Products
                .Where(p => p.Name.ToLower().Contains(query.ToLower()))
                .Select(p => new { p.Id, p.Name })
                .Take(5)
                .ToListAsync();
            return Json(products);
        }

        /*
          Filters products based on price range, category, status, and stock availability.
          Parameters:
            minPrice   - The minimum price of products (optional).
            maxPrice   - The maximum price of products (optional).
            categoryId - The ID of the category to filter by (optional).
            status     - The status of the product to filter by (optional).
            inStock    - A boolean indicating whether to filter in-stock products (optional).
          Returns: A view displaying the filtered products along with categories and statuses.
        */
        public async Task<IActionResult> FilterProducts(int? minPrice, int? maxPrice, int? categoryId, int? status, bool? inStock)
        {
            List<Product> products = await _context.Products.Include(p => p.Category).ToListAsync();
            var filteredProducts = products.AsQueryable();
            if (minPrice.HasValue)
                filteredProducts = filteredProducts.Where(p => p.Price >= minPrice.Value);
            if (maxPrice.HasValue)
                filteredProducts = filteredProducts.Where(p => p.Price <= maxPrice.Value);
            if (categoryId.HasValue)
                filteredProducts = filteredProducts.Where(p => p.Category.Id == categoryId.Value);
            if (status.HasValue)
                filteredProducts = filteredProducts.Where(p => p.Status == status.Value);
            if (inStock.HasValue)
                filteredProducts = filteredProducts.Where(p => inStock.Value ? p.Quantity > 0 : p.Quantity == 0);
            var viewModel = new ProductViewModel
            {
                Products = filteredProducts.ToList(),
                Categories = await GetCategories(),
                ProductStatuses = await GetProductStatuses()
            };
            return View("Product", viewModel);
        }
    }
}
