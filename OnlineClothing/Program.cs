using Microsoft.AspNetCore.Http.Features;
using Microsoft.EntityFrameworkCore;
using OnlineClothing.Models;
using OnlineClothing.Services;
using OnlineClothing.Utils;

var builder = WebApplication.CreateBuilder(args);

// Configure the DbContext to use SQL Server
builder.Services.AddDbContext<ClothingShopPrn222G2Context>(options =>
    options.UseSqlServer(builder.Configuration.GetConnectionString("DbConnection")));

builder.Services.AddScoped<IFileUploadService, FileUploadService>();

// Register the EmailUtils as a transient service
builder.Services.AddTransient<EmailUtils>();

// Add services to the container for MVC Controllers and Views
builder.Services.AddControllersWithViews();

// Configure session settings
builder.Services.AddDistributedMemoryCache(); // Necessary for session storage
builder.Services.AddSession(options =>
{
    options.IdleTimeout = TimeSpan.FromMinutes(30); // Set session timeout to 30 minutes
    options.Cookie.HttpOnly = true;  // Makes the session cookie HttpOnly
    options.Cookie.IsEssential = true;  // Ensures cookies are essential for the app
});

builder.Services.Configure<FormOptions>(options =>
{
    options.ValueLengthLimit = int.MaxValue;
    options.MultipartBodyLengthLimit = 104857600;
});


var app = builder.Build();

// Apply the session middleware
app.UseSession();

// Configure the HTTP request pipeline
if (!app.Environment.IsDevelopment())
{
    app.UseExceptionHandler("/Home/Error"); // Error handling in production
}
app.UseStaticFiles(); // Serve static files (e.g., CSS, JavaScript)

app.UseRouting(); // Routing configuration

app.UseAuthorization(); // Authorization middleware

// Configure MVC route (default route)
app.MapControllerRoute(
    name: "default",
    pattern: "{controller=Home}/{action=Index}/{id?}");

app.Run();

