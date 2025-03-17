using Microsoft.AspNetCore.Http.Features;
using Microsoft.EntityFrameworkCore;
using OnlineClothing.Models;
using OnlineClothing.Services;
using OnlineClothing.Utils;

var builder = WebApplication.CreateBuilder(args);

builder.Logging.ClearProviders();
builder.Logging.AddConsole();

builder.Services.AddDbContext<ClothingShopPrn222G2Context>(options =>
    options.UseSqlServer(builder.Configuration.GetConnectionString("DbConnection")));

builder.Services.AddScoped<IFileUploadService, FileUploadService>();
builder.Services.AddSingleton<IOpenAIService, OpenAIService>();

// Add services to the container.
builder.Services.AddTransient<EmailUtils>();

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

app.UseStatusCodePagesWithRedirects("/Error/{0}");

// Apply the session middleware
app.UseSession(); 
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