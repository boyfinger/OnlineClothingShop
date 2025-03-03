using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using OnlineClothing.Models;
using OnlineClothing.Utils;
using System.CodeDom.Compiler;
using System.Text;

namespace OnlineClothing.Controllers
{
    public class AccountController : Controller
    {
        private readonly ClothingShopPrn222G2Context context;
        private readonly EmailUtils emailUtils;

        public AccountController(ClothingShopPrn222G2Context context, EmailUtils emailUtils)
        {
            this.context = context;
            this.emailUtils = emailUtils;
        }

        [HttpGet]
        public IActionResult Login()
        {
            var userId = HttpContext.Session.GetString("UserId");
            if (userId != null)
            {
                var userRole = HttpContext.Session.GetString("UserRole");
                if (userRole == "SELLER")
                {
                    return RedirectToAction("Index", "SellerProducts");
                }
                else if (userRole == "CUSTOMER")
                {
                    return RedirectToAction("Index", "Home");
                }
                else if (userRole == "ADMIN")
                {
                    return RedirectToAction("Index", "Home");
                }
            }
            return View();
        }

        public async Task<IActionResult> Login(LoginViewModel model)
        {
            if (!ModelState.IsValid)
            {
                return View(model);
            }
            var user = await context.Users.FirstOrDefaultAsync(u => u.Email == model.LoginEmail);
            if (user == null)
            {
                ModelState.AddModelError(string.Empty, "Invalid email or password.");
                return View(model);
            }
            int failedAttempts = HttpContext.Session.GetInt32($"FailedAttempts_{model.LoginEmail}") ?? 0;
            if (failedAttempts >= 5)
            {
                ModelState.AddModelError(string.Empty, "Your account is temporarily locked due to multiple failed login attempts.");
                return View(model);
            }
            if (user.Password != model.LoginPassword)
            {
                HttpContext.Session.SetInt32($"FailedAttempts_{model.LoginEmail}", failedAttempts + 1);
                if (failedAttempts + 1 >= 5)
                {
                    user.Status = 3;
                    await context.SaveChangesAsync();

                    ModelState.AddModelError(string.Empty, "Your account has been banned due to too many failed login attempts.");
                }
                else
                {
                    ModelState.AddModelError(string.Empty, "Invalid email or password.");
                }
                return View(model);
            }
            HttpContext.Session.SetInt32($"FailedAttempts_{model.LoginEmail}", 0);
            if (user.Status == 2)
            {
                string token = GenerateVerificationToken(model.LoginEmail);
                string verificationLink = $"http://localhost:5222/Account/VerifyEmail?token={token}";
                string subject = "Please Verify Your Email Address";
                string body = $"Dear {model.LoginEmail},\n\nPlease click the following link to verify your email address: {verificationLink}";
                await emailUtils.SendEmailAsync(model.LoginEmail, subject, body);
                return RedirectToAction("Verify");
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
            var userInfo = await context.Userinfos.FirstOrDefaultAsync(ui => ui.Id == user.Id);
            if (userInfo != null)
            {
                HttpContext.Session.SetString("AvatarUrl", userInfo.AvatarUrl ?? "/images/default-avatar.jpg"); // Set default if no avatar
                Console.WriteLine(HttpContext.Session.GetString("AvatarUrl"));
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
            var userId = HttpContext.Session.GetString("UserId");
            if (userId != null)
            {
                var userRole = HttpContext.Session.GetString("UserRole");
                if (userRole == "ADMIN")
                {
                    return RedirectToAction("Index", "AdminDashboard");
                }
                else
                {
                    return RedirectToAction("Index", "Home");
                }
            }
            return View();
        }

        [HttpPost]
        public async Task<IActionResult> AdminLogin(LoginViewModel model)
        {
            if (!ModelState.IsValid)
            {
                return View(model);
            }
            var adminUser = await context.Users.FirstOrDefaultAsync(u => u.Email == model.LoginEmail && u.Password == model.LoginPassword);
            if (adminUser == null)
            {
                ModelState.AddModelError(string.Empty, "Wrong email or password.");
                return View(model);
            }
            var userRoles = await context.UserRoles.Where(ur => ur.UserId == adminUser.Id).Select(ur => ur.RoleId).ToListAsync();
            var userRole = await context.UserRoles.Where(ur => ur.UserId == adminUser.Id && ur.RoleId == 1).FirstOrDefaultAsync();
            bool isAdmin = userRoles.Contains(1);
            if (!isAdmin)
            {
                ModelState.AddModelError(string.Empty, "You are not an admin.");
                return View(model); ;
            }
            HttpContext.Session.SetString("UserId", adminUser.Id.ToString());
            HttpContext.Session.SetString("UserRole", "ADMIN");
            return RedirectToAction("Index", "Home");
        }

        [HttpGet]
        public IActionResult SignUp()
        {
            var userId = HttpContext.Session.GetString("UserId");
            if (userId != null)
            {
                var userRole = HttpContext.Session.GetString("UserRole");
                if (userRole == "SELLER")
                {
                    return RedirectToAction("Index", "SellerProducts");
                }
                else if (userRole == "CUSTOMER")
                {
                    return RedirectToAction("Index", "Home");
                }
                else if (userRole == "ADMIN")
                {
                    return RedirectToAction("Index", "Home");
                }
            }
            return View();
        }

        public async Task<IActionResult> SignUp(SignUpViewModel model)
        {
            string validationError = ValidationUser(model);
            if (validationError != null)
            {
                ModelState.AddModelError(string.Empty, validationError);
                return View(model);
            }
            var existingUserByEmail = await context.Users
                .FirstOrDefaultAsync(u => u.Email == model.Email);
            if (existingUserByEmail != null)
            {
                ModelState.AddModelError(string.Empty, "An account with this email already exists.");
                return View(model);
            }
            var existingUserByUsername = await context.Users
                .FirstOrDefaultAsync(u => u.UserName == model.UserName);
            if (existingUserByUsername != null)
            {
                ModelState.AddModelError(string.Empty, "An account with this username already exists.");
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
                AvatarUrl = "/images/default-avatar.jpg",
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
                    RoleId = 2
                });
            }
            await context.SaveChangesAsync();
            HttpContext.Session.SetString("UserId", newUser.Id.ToString());
            HttpContext.Session.SetString("UserRole", model.UserType.ToUpper());
            HttpContext.Session.SetString("AvatarUrl", userInfo.AvatarUrl);
            string token = GenerateVerificationToken(newUser.Email);
            string verificationLink = $"http://localhost:5222/Account/VerifyEmail?token={token}";
            string subject = "Please Verify Your Email Address";
            string body = $"Dear {model.FullName},\n\nPlease click the following link to verify your email address: {verificationLink}";
            await emailUtils.SendEmailAsync(model.Email, subject, body);
            return RedirectToAction("Verify");
        }

        [HttpGet]
        public IActionResult VerifyEmail(string token)
        {
            try
            {
                var email = DecodeVerificationToken(token);
                var user = context.Users.FirstOrDefault(u => u.Email == email);
                if (user == null)
                {
                    return View("NotFound");
                }
                var expirationDate = GetTokenExpirationDate(token);
                if (DateTime.UtcNow > expirationDate)
                {
                    return View("TokenExpired");
                }
                user.Status = 1;
                context.SaveChanges();
                return View("Verified");
            }
            catch (Exception)
            {
                return View("NotFound");
            }
        }

        [HttpPost]
        public IActionResult Logout()
        {
            HttpContext.Session.Clear();
            return RedirectToAction("Index", "Home");
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
            if (!ValidationUtils.IsValidUserName(model.UserName))
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

        private string GenerateVerificationToken(string email)
        {
            string token = Convert.ToBase64String(Encoding.UTF8.GetBytes(email + "&" + DateTime.UtcNow.AddMinutes(30).ToString()));
            return token;
        }

        private string DecodeVerificationToken(string token)
        {
            var tokenData = Encoding.UTF8.GetString(Convert.FromBase64String(token));
            var email = tokenData.Split('&')[0];
            return email;
        }

        private DateTime GetTokenExpirationDate(string token)
        {
            var tokenData = Encoding.UTF8.GetString(Convert.FromBase64String(token));
            var expirationDate = DateTime.Parse(tokenData.Split('&')[1]);
            return expirationDate;
        }

        [HttpGet]
        public IActionResult Verify()
        {
            var userId = HttpContext.Session.GetString("UserId");
            if (userId == null)
            {
                return RedirectToAction("NotFound");
            }
            return View();
        }

        [HttpGet]
        public IActionResult Verified()
        {
            var userId = HttpContext.Session.GetString("UserId");
            if (userId == null)
            {
                return RedirectToAction("NotFound");
            }
            return View();
        }

        [HttpGet]
        public IActionResult TokenExpired()
        {
            var userId = HttpContext.Session.GetString("UserId");
            if (userId == null)
            {
                return RedirectToAction("NotFound");
            }
            return View();
        }

        [HttpGet]
        public IActionResult NotFound()
        {
            return View();
        }

    }
}
