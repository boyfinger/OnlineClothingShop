using Microsoft.EntityFrameworkCore;
using OnlineClothing.Models;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddDbContext<ClothingShopPrn222G2Context>(options =>
    options.UseSqlServer(builder.Configuration.GetConnectionString("DbConnection")));

// Add services to the container.
builder.Services.AddControllersWithViews();

builder.Services.AddSession(options =>
{
    options.IdleTimeout = TimeSpan.FromMinutes(30);
    options.Cookie.HttpOnly = true;
    options.Cookie.IsEssential = true;
});

builder.Services.AddDistributedMemoryCache();

var app = builder.Build();

app.UseSession();

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

