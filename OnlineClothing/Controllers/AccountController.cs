using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using OnlineClothing.Models;

namespace OnlineClothing.Controllers
{
    public class AccountController : Controller
    {
        private readonly ClothingShopPrn222G2Context context;

        public AccountController(ClothingShopPrn222G2Context context)
        {
            this.context = context;
        }

        [HttpGet]
        public IActionResult Login()
        {
            return View();
        }

        [HttpPost]
        public async Task<IActionResult> Login(AccountViewModel model)
        {
            if (!ModelState.IsValid)
            {
                return View(model);
            }

            // Check if user exists in the database
            var user = await context.Users
                .FirstOrDefaultAsync(u => u.Email == model.LoginEmail && u.Password == model.LoginPassword);

            if (user == null)
            {
                ModelState.AddModelError(string.Empty, "Invalid email or password");
                return View(model);
            }

            // Check user status
            if (user.Status == 2)
            {
                ModelState.AddModelError(string.Empty, "Your account is not verified yet.");
                return View(model);
            }
            if (user.Status == 3)
            {
                ModelState.AddModelError(string.Empty, "Your account has been banned.");
                return View(model);
            }

            var userRoles = await context.UserRoles
                .Where(ur => ur.UserId == user.Id)
                .Select(ur => ur.RoleId)
                .ToListAsync();

            bool isSeller = userRoles.Contains(2);
            bool isCustomer = userRoles.Contains(3);

            // Correct error messages based on selected user type
            if (model.UserType == "seller" && !isSeller)
            {
                ModelState.AddModelError(string.Empty, "You are not a seller.");
                return View(model);
            }
            if (model.UserType == "customer" && !isCustomer)
            {
                ModelState.AddModelError(string.Empty, "You are not a customer.");
                return View(model);
            }

            // Store user session
            HttpContext.Session.SetString("UserId", user.Id.ToString());
            HttpContext.Session.SetString("UserRole", model.UserType.ToUpper());

            return RedirectToAction("Index", "Home");
        }

        [HttpGet]
        public IActionResult AdminLogin()
        {
            return View();
        }


        [HttpPost]
        public async Task<IActionResult> AdminLogin(AccountViewModel model)
        {
            if (!ModelState.IsValid)
            {
                return View(model);
            }

            // Check if the user exists in the database
            var adminUser = await context.Users
                .FirstOrDefaultAsync(u => u.Email == model.LoginEmail && u.Password == model.LoginPassword);

            if (adminUser == null)
            {
                ModelState.AddModelError(string.Empty, "Wrong email or password.");
                return View(model);
            }

            // Check if the user has the admin role (role_id 1 for admin)
            var userRoles = await context.UserRoles
                .Where(ur => ur.UserId == adminUser.Id)
                .Select(ur => ur.RoleId)
                .ToListAsync();

            var userRole = await context.UserRoles.Where(ur => ur.UserId == adminUser.Id && ur.RoleId == 1).FirstOrDefaultAsync();

            bool isAdmin = userRoles.Contains(1); // Assuming 1 corresponds to "admin" role

            if (!isAdmin)
            {

                ModelState.AddModelError(string.Empty, "You are not an admin.");
                return View(model); ;
            }

            // Store the session for admin
            HttpContext.Session.SetString("UserId", adminUser.Id.ToString());
            HttpContext.Session.SetString("UserRole", "ADMIN");

            return RedirectToAction("Index", "Home");
        }

        [HttpPost]
        public IActionResult Logout()
        {
            HttpContext.Session.Clear();
            return RedirectToAction("Login");
        }
    }
}
