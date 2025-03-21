using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Caching.Memory;
using OnlineClothing.Models;
using System.Linq;

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
                    .Where(p => p.Status == 1)
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
                ViewData["ActionName"] = "Index";
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
                .FirstOrDefaultAsync(p => p.Id == id);
            if (product == null)
            {
                return RedirectToAction("HandleError", "Error", new { statusCode = 404 });
            }
            var relatedProducts = await _context.Products
                .Where(p => p.Status == 1)
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
            var feedbacks = await _context.Feedbacks
                .Where(f => f.ProductId == id)
                .Include(f => f.User)
                .ThenInclude(f => f.Userinfo)
                .OrderByDescending(f => f.CreateAt)
                .Take(3)
                .ToListAsync();
            var viewModel = new ProductDetailViewModel
            {
                Product = product,
                RelatedProducts = relatedProducts,
                Feedbacks = feedbacks
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
                    .Where(p => p.Status == 1)
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
                ViewData["ActionName"] = "Search";
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
                .Where(p => p.Status == 1)
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
            sortOrder  - The sort type(optional).
          Returns: A view displaying the filtered products along with categories and statuses.
        */
        public async Task<IActionResult> FilterProducts(int? minPrice, int? maxPrice, int? categoryId, string sortOrder, int? status, int page = 1, int pageSize = 8)
        {
            ViewBag.MinPrice = minPrice;
            ViewBag.MaxPrice = maxPrice;
            ViewBag.CategoryId = categoryId;
            ViewBag.Status = status;
            var validStatuses = new HashSet<int> { 1, 2 };
            if (status.HasValue && !validStatuses.Contains(status.Value))
            {
                return RedirectToAction("HandleError", "Error", new { statusCode = 400, errorMessage = "Trạng thái sản phẩm không hợp lệ!" });
            }
            IQueryable<Product> filteredProducts = _context.Products.Include(p => p.Category);
            if (minPrice.HasValue)
                filteredProducts = filteredProducts.Where(p => (p.Price * (1 - p.Discount / 100.0)) >= minPrice.Value);
            if (maxPrice.HasValue)
                filteredProducts = filteredProducts.Where(p => (p.Price * (1 - p.Discount / 100.0)) <= maxPrice.Value);
            if (categoryId.HasValue)
                filteredProducts = filteredProducts.Where(p => p.Category.Id == categoryId.Value);
            if (status.HasValue)
            {
                if (status == 1)
                {
                    filteredProducts = filteredProducts.Where(p => p.Quantity > 0);
                }
                else if (status == 2)
                {
                    filteredProducts = filteredProducts.Where(p => p.Quantity == 0);
                }
            }
            switch (sortOrder)
            {
                case "price_asc":
                    filteredProducts = filteredProducts.OrderBy(p => (p.Price * (1 - p.Discount / 100.0)));
                    break;
                case "price_desc":
                    filteredProducts = filteredProducts.OrderByDescending(p => (p.Price * (1 - p.Discount / 100.0)));
                    break;
                default:
                    filteredProducts = filteredProducts.OrderBy(p => p.Id);
                    break;
            }
            int totalProducts = await filteredProducts.CountAsync();
            List<Product> PagingProducts = await filteredProducts
                .Where(p => p.Status == 1)
                    .Skip((page - 1) * pageSize)
                    .Take(pageSize)
                    .ToListAsync();
            var viewModel = new ProductViewModel
            {
                Products = PagingProducts,
                Categories = await GetCategories(),
                ProductStatuses = await GetProductStatuses(),
                CurrentPage = page,
                TotalPages = (int)Math.Ceiling(totalProducts / (double)pageSize),
                SearchQuery = null
            };
            ViewData["ActionName"] = "FilterProducts";
            ViewData["MinPrice"] = minPrice;
            ViewData["MaxPrice"] = maxPrice;
            ViewData["CategoryId"] = categoryId;
            ViewData["Status"] = status;
            ViewData["SortOrder"] = sortOrder;
            return View("Product", viewModel);
        }
    }
}
