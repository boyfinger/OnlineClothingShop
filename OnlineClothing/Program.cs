using Microsoft.EntityFrameworkCore;
using OnlineClothing.Models;
using OnlineClothing.Services;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddDbContext<ClothingShopPrn222G2Context>(options =>
    options.UseSqlServer(builder.Configuration.GetConnectionString("DbConnection")));

builder.Services.AddScoped<IFileUploadService, FileUploadService>();

// Add services to the container.
builder.Services.AddControllersWithViews();

var app = builder.Build();

// Configure the HTTP request pipeline.
if (!app.Environment.IsDevelopment())
{
    app.UseExceptionHandler("/Home/Error");
}
app.UseStaticFiles();

app.UseRouting();

app.UseAuthorization();

builder.Services.AddMemoryCache();

app.MapControllerRoute(
    name: "default",
    pattern: "{controller=Home}/{action=Index}/{id?}");

app.Run();