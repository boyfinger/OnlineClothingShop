using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Caching.Memory;
using OnlineClothing.Controllers;
using OnlineClothing.Models;


namespace MyAspNetMvcProject.Tests.tests
{
    public class ProductControllerTests
    {
        private ClothingShopPrn222G2Context InitContextAndData()
        {
            var options = new DbContextOptionsBuilder<ClothingShopPrn222G2Context>()
                .UseInMemoryDatabase(databaseName: "TestDatabase")
                .Options;
            using (var context = new ClothingShopPrn222G2Context(options))
            {
                var categories = new List<Category>
                {
                    new Category { Id = 1, Name = "Category A" },
                    new Category { Id = 2, Name = "Category B" }
                };
                var status = new List<ProductStatus>
                {
                    new ProductStatus { Id = 1, Name = "Available" },
                    new ProductStatus { Id = 2, Name = "Out of Stock" }
                };

                var sellerid = Guid.NewGuid();
                var seller = new User { Id = sellerid, UserName = "Seller 1", Userinfo = new Userinfo { Id = sellerid, FullName = "User A" } };
                var product = new Product
                {
                    Id = 1,
                    Name = "Product A",
                    Description = "Desc A",
                    Price = 100000,
                    ThumbnailUrl = "image.jpg",
                    Discount = 10,
                    Status = 1,
                    Quantity = 1,
                    Category = categories.FirstOrDefault(),
                    Seller = seller,
                    Images = new List<Image>
                    {
                        new Image { Id = 1, Url = "img1.jpg" }
                    }
                };
                var relatedProduct = new Product
                {
                    Id = 2,
                    Name = "Related Product",
                    Description = "Desc B",
                    Price = 150000,
                    ThumbnailUrl = "image2.jpg",
                    Discount = 5,
                    Quantity = 10,
                    Status = 1,
                    Category = categories.FirstOrDefault(),
                    Seller = seller
                };
                var userid = Guid.NewGuid();
                var feedback = new Feedback
                {
                    Id = 1,
                    ProductId = 1,
                    Comment = "Great product!",
                    CreateAt = System.DateTime.UtcNow,
                    User = new User
                    {
                        Id = userid,
                        UserName = "userB",
                        Userinfo = new Userinfo { Id = userid, FullName = "User B" }
                    }
                };
                context.Categories.AddRange(categories);
                context.ProductStatuses.AddRange(status);
                context.Products.AddRange(product, relatedProduct);
                context.Feedbacks.Add(feedback);
                context.SaveChangesAsync();
            }
            return new ClothingShopPrn222G2Context(options);
        }

        [Fact]
        public async Task GetCategories_FetchesFromDatabaseAndCaches()
        {
            // Arrange
            var context = InitContextAndData();
            var memoryCache = new MemoryCache(new MemoryCacheOptions());
            // Act
            List<Category> categories;
            var controller = new ProductController(context, memoryCache);
            categories = await controller.GetCategories();
            // Assert
            Assert.Equal(2, categories.Count);
            Assert.True(memoryCache.TryGetValue("categories", out _));
        }

        [Fact]
        public async Task GetProductStatuses_FetchesFromDatabaseAndCaches()
        {
            // Arrange
            var context = InitContextAndData();
            var memoryCache = new MemoryCache(new MemoryCacheOptions());
            // Act
            List<ProductStatus> statuses;
            var controller = new ProductController(context, memoryCache);
            statuses = await controller.GetProductStatuses();
            // Assert
            Assert.Equal(2, statuses.Count);
            Assert.True(memoryCache.TryGetValue("statuses", out _));
        }

        [Fact]
        public async Task Detail_ReturnsViewWithValidProduct()
        {
            // Arrange
            var context = InitContextAndData();
            var memoryCache = new MemoryCache(new MemoryCacheOptions());
            // Act
            var controller = new ProductController(context, memoryCache);
            var result = await controller.Detail(1);
            // Assert
            var actionResult = Assert.IsType<ActionResult<IEnumerable<Product>>>(result);
            var viewResult = Assert.IsType<ViewResult>(actionResult.Result);
            var model = Assert.IsType<ProductDetailViewModel>(viewResult.Model);
            Assert.NotNull(model.Product);
            Assert.Equal("Product A", model.Product.Name);
            Assert.Equal(2, model.RelatedProducts.Count);
            Assert.Single(model.Feedbacks);
        }
    }
}
