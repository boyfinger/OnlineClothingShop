using System.Text.Json;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using MyAspNetMvcProject.Tests.utils;
using OnlineClothing.Controllers;
using OnlineClothing.Models;

namespace MyAspNetMvcProject.Tests.tests
{
    public class CartControllerTests : IAsyncLifetime
    {
        private readonly ClothingShopPrn222G2Context _context;
        private readonly CartController _controller;
        private readonly Guid userId = Guid.NewGuid();
        public CartControllerTests()
        {
            // Configure in-memory database
            var options = new DbContextOptionsBuilder<ClothingShopPrn222G2Context>()
                .UseInMemoryDatabase(databaseName: Guid.NewGuid().ToString()) // Unique DB per test run
                .Options;

            _context = new ClothingShopPrn222G2Context(options);

            var httpContext = new DefaultHttpContext
            {
                Session = new FakeSession() // FakeSession replaces Moq usage
            };
            _controller = new CartController(_context)
            {
                ControllerContext = new ControllerContext
                {
                    HttpContext = httpContext
                }
            };

            SeedData();  // Seed the database with test data
        }

        private void SeedData()
        {
            var cart = new Cart
            {
                Id = Guid.NewGuid(),
                UserId = userId,
                TotalAmount = 100,
                CartDetails = new List<CartDetail>
                {
                    new CartDetail { ProductId = 1, Quantity = 1, TotalPrice = 100 }
                }
            };
            var product = new Product { Id = 1, Description = "Test Product Description", Name = "Test Product", Quantity = 10, Price = 100 };

            _context.Users.Add(new User { Id = userId });
            _context.Products.Add(product);
            _context.Carts.Add(cart);
            _context.SaveChanges();
        }

        [Fact]
        public async Task AddToCart_ValidProduct_AddsToCartSuccessfully()
        {
            // Arrange
            var productId = 1;
            _controller.HttpContext.Session.SetString("UserId", Guid.NewGuid().ToString()); // Mock UserId

            // Act
            var result = await _controller.AddToCart(productId, 2);

            // Assert
            var jsonResult = Assert.IsType<JsonResult>(result);

            // Deserialize the JSON result into a dictionary
            var jsonData = JsonSerializer.Deserialize<Dictionary<string, object>>(JsonSerializer.Serialize(jsonResult.Value));
            Assert.NotNull(jsonData);
            Assert.Equal("Product added to cart successfully!", jsonData["message"].ToString());
        }

        [Fact]
        public async Task Remove_ExistingProduct_RemovesFromCart()
        {
            // Arrange
            var productId = 1;
            _controller.HttpContext.Session.SetString("UserId", userId.ToString()); // Mock UserId

            // Act
            var result = await _controller.Remove(productId);

            // Assert
            var jsonResult = Assert.IsType<JsonResult>(result);

            // Deserialize JSON and extract properties safely from JsonElement
            var jsonData = JsonSerializer.Deserialize<Dictionary<string, JsonElement>>(JsonSerializer.Serialize(jsonResult.Value));
            Assert.NotNull(jsonData);

            // Access the "success" property from JsonElement and convert it to bool
            Assert.True(jsonData["success"].GetBoolean());
        }

        [Fact]
        public async Task UpdateQuantity_ValidProduct_UpdatesQuantity()
        {
            // Arrange
            var productId = 1;
            _controller.HttpContext.Session.SetString("UserId", userId.ToString()); // Mock UserId

            // Act
            var result = await _controller.UpdateQuantity(productId, 5);

            // Assert
            var jsonResult = Assert.IsType<JsonResult>(result);

            // Deserialize JSON and extract properties safely from JsonElement
            var jsonData = JsonSerializer.Deserialize<Dictionary<string, JsonElement>>(JsonSerializer.Serialize(jsonResult.Value));
            Assert.NotNull(jsonData);

            // Access the "success" property from JsonElement and convert it to bool
            Assert.True(jsonData["success"].GetBoolean());
        }
        // Cleanup the database after each test run
        public Task InitializeAsync() => Task.CompletedTask;

        public Task DisposeAsync()
        {
            _context.Database.EnsureDeleted(); // Deletes the in-memory DB after tests run
            return Task.CompletedTask;
        }
    }
}
