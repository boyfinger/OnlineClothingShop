using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using OnlineClothing.Models;
using OnlineClothing.Utils;
using System.CodeDom.Compiler;

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
        public async Task<IActionResult> Login(LoginViewModel model)
        {
            if (!ModelState.IsValid)
            {
                return View(model);
            }
            var user = await context.Users
                .FirstOrDefaultAsync(u => u.Email == model.LoginEmail && u.Password == model.LoginPassword);
            if (user == null)
            {
                ModelState.AddModelError(string.Empty, "Invalid email or password.");
                return View(model);
            }
            if (user.Status == 2)
            {
                ModelState.AddModelError(string.Empty, "Your account is not verified yet.");
                return View(model);
            }
            if (user.Status == 3)
            {
                ModelState.AddModelError(string.Empty, "Your account has been banned. Please contact admin.");
                return View(model);
            }
            var userRoles = await context.UserRoles
                .Where(ur => ur.UserId == user.Id)
                .Select(ur => ur.RoleId)
                .ToListAsync();
            bool isSeller = userRoles.Contains(2);
            bool isCustomer = userRoles.Contains(3);
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
            HttpContext.Session.SetString("UserId", user.Id.ToString());
            HttpContext.Session.SetString("UserRole", model.UserType.ToUpper());
            if (isCustomer)
            {
                return RedirectToAction("Index", "Home");
            }
            else
            {
                return RedirectToAction("Index", "SellerProducts");
            }
        }

        [HttpGet]
        public IActionResult AdminLogin()
        {
            return View();
        }

        [HttpPost]
        public async Task<IActionResult> AdminLogin(LoginViewModel model)
        {
            if (!ModelState.IsValid)
            {
                return View(model);
            }
            
            var adminUser = await context.Users
                .FirstOrDefaultAsync(u => u.Email == model.LoginEmail && u.Password == model.LoginPassword);

            if (adminUser == null)
            {
                ModelState.AddModelError(string.Empty, "Wrong email or password.");
                return View(model);
            }

            var userRoles = await context.UserRoles
                .Where(ur => ur.UserId == adminUser.Id)
                .Select(ur => ur.RoleId)
                .ToListAsync();

            var userRole = await context.UserRoles.Where(ur => ur.UserId == adminUser.Id && ur.RoleId == 1).FirstOrDefaultAsync();

            bool isAdmin = userRoles.Contains(1);

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


        [HttpGet]
        public IActionResult SignUp()
        {
            return View();
        }
        [HttpPost]
        public async Task<IActionResult> SignUp(SignUpViewModel model)
        {
            string validationError = ValidationUser(model);
            if (validationError != null)
            {
                ModelState.AddModelError(string.Empty, validationError);
                return View(model);
            }
            var newUser = new User
            {
                Id = Guid.NewGuid(),
                UserName = model.UserName,
                Email = model.Email,
                Password = model.Password,
                Status = 2,
                CreatedAt = DateTime.UtcNow
            };
            context.Users.Add(newUser);
            await context.SaveChangesAsync();
            var userInfo = new Userinfo
            {
                Id = newUser.Id,
                FullName = model.FullName,
                PhoneNumber = model.PhoneNumber,
                AvatarUrl = "https://static.vecteezy.com/system/resources/previews/009/292/244/non_2x/default-avatar-icon-of-social-media-user-vector.jpg",
                Gender = model.Gender == "male" ? 1 : model.Gender == "female" ? 2 : 3,
                DateOfBirth = model.DateOfBirth,
                Address = model.Address,
                UpdateAt = DateTime.UtcNow
            };
            context.Userinfos.Add(userInfo);
            await context.SaveChangesAsync();
            if (model.UserType == "customer")
            {
                context.UserRoles.Add(new UserRole
                {
                    UserId = newUser.Id,
                    RoleId = 3
                });
            }
            else if (model.UserType == "seller")
            {
                context.UserRoles.Add(new UserRole
                {
                    UserId = newUser.Id,
                    RoleId = 2,
                });
            }
            await context.SaveChangesAsync();
            return RedirectToAction("Verify");
        }



        [HttpPost]
        public IActionResult Logout()
        {
            HttpContext.Session.Clear();
            return RedirectToAction("Login");
        }

        public string ValidationUser(SignUpViewModel model)
        {
            if (!ValidationUtils.IsValidFullName(model.FullName))
            {
                return "Full Name can not over 100 characters. Contains only uppercase, lowercase and space";
            }
            if (!ValidationUtils.IsValidEmail(model.Email))
            {
                return "Please enter valid email!";
            }
            if(!ValidationUtils.IsValidUserName(model.UserName))
            {
                return "Please enter valid username! Only alphabet, number and underscore.";
            }
            if (!ValidationUtils.IsValidPassword(model.Password))
            {
                return "Password contains at least one uppercase, one lowercase, one digit. Length >= 8";
            }
            if (!ValidationUtils.IsValidMobile(model.PhoneNumber))
            {
                return "Please enter valid phone number.";
            }
            if (!ValidationUtils.IsValidGender(model.Gender))
            {
                return "Please choose a gender.";
            }
            if (!ValidationUtils.IsValidDateOfBirth(model.DateOfBirth))
            {
                return "You must be older than 18 years old.";
            }
            return null;
        }
    }
}
