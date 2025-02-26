using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using OnlineClothing.Models;
using OnlineClothing.Utils;

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
            var model = new AccountViewModel
            {
                LoginModel = new LoginViewModel(),
                SignupModel = new SignUpViewModel()
            };

            return View(model);
        }

        [HttpPost]
        public async Task<IActionResult> Login(AccountViewModel model)
        {
            if (!ModelState.IsValid)
            {
                return View(model);
            }

            // Fetch the user based on the email
            var user = await context.Users
                .FirstOrDefaultAsync(u => u.Email == model.LoginModel.LoginEmail && u.Password == model.LoginModel.LoginPassword);

            if (user == null)
            {
                ModelState.AddModelError(string.Empty, "Invalid email or password.");
                return View(model);
            }

            // Check user status (if active, banned, or unverified)
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

            // Check roles (customer or seller)
            var userRoles = await context.UserRoles
                .Where(ur => ur.UserId == user.Id)
                .Select(ur => ur.RoleId)
                .ToListAsync();

            bool isSeller = userRoles.Contains(2);
            bool isCustomer = userRoles.Contains(3);

            // Validate user type selection
            if (model.LoginModel.UserType == "seller" && !isSeller)
            {
                ModelState.AddModelError(string.Empty, "You are not a seller.");
                return View(model);
            }
            if (model.LoginModel.UserType == "customer" && !isCustomer)
            {
                ModelState.AddModelError(string.Empty, "You are not a customer.");
                return View(model);
            }

            // Store session data after successful login
            HttpContext.Session.SetString("UserId", user.Id.ToString());
            HttpContext.Session.SetString("UserRole", model.LoginModel.UserType.ToUpper());

            return RedirectToAction("Index", "Home"); // Redirect to homepage/dashboard after successful login
        }


        [HttpGet]
        public IActionResult AdminLogin()
        {
            var model = new AccountViewModel
            {
                LoginModel = new LoginViewModel(),
                SignupModel = new SignUpViewModel()
            };

            return View(model);
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
                .FirstOrDefaultAsync(u => u.Email == model.LoginModel.LoginEmail && u.Password == model.LoginModel.LoginPassword);

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
        public async Task<IActionResult> SignUp(SignUpViewModel model)
        {
            // Validate user input
            string validationError = ValidationUser(model);

            if (validationError != null)
            {
                ModelState.AddModelError(string.Empty, validationError);
                return View(model);
            }

            // Create a new User (without hashing the password)
            var newUser = new User
            {
                Id = Guid.NewGuid(), // Generate new GUID for User ID
                Email = model.Email,
                Password = model.Password, // Store the password as plain text (not recommended for production)
                Status = 1, // Set the status as needed
                CreatedAt = DateTime.UtcNow
            };

            // Add the User to the database
            context.Users.Add(newUser);
            await context.SaveChangesAsync();

            // Create the UserInfo record (personal information like FullName, PhoneNumber)
            var userInfo = new Userinfo
            {
                Id = newUser.Id,
                FullName = model.FullName,
                PhoneNumber = model.PhoneNumber,
                Gender = model.Gender == "male" ? 1 : model.Gender == "female" ? 2 : 3,
                DateOfBirth = model.DateOfBirth,
                Address = model.Address,
                UpdateAt = DateTime.UtcNow
            };

            // Add the UserInfo to the database
            context.Userinfos.Add(userInfo);
            await context.SaveChangesAsync();

            // Add roles to the User (assuming "3" = Customer and "2" = Seller)
            if (model.UserType == "customer")
            {
                context.UserRoles.Add(new UserRole
                {
                    UserId = newUser.Id,
                    RoleId = 3 // 3 = Customer role
                });
            }
            else if (model.UserType == "seller")
            {
                context.UserRoles.Add(new UserRole
                {
                    UserId = newUser.Id,
                    RoleId = 2 // 2 = Seller role
                });
            }

            // Save roles in the UserRoles table
            await context.SaveChangesAsync();

            // Redirect to the login page after successful sign-up
            return RedirectToAction("Login");
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
