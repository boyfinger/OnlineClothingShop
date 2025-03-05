using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using OnlineClothing.Models;
using OnlineClothing.Utils;
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

        //Check UserId, if they are already logged in, redirect them to their home page
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

            // Check if user exists using either email or username
            var user = await context.Users
                                    .FirstOrDefaultAsync(u => u.Email == model.LoginUserName || u.UserName == model.LoginUserName);

            // Check if user is found in the database
            if (user == null)
            {
                ModelState.AddModelError(string.Empty, "Invalid email/username or password.");
                return View(model);
            }

            int failedAttempts = HttpContext.Session.GetInt32($"FailedAttempts_{model.LoginUserName}") ?? 0;
            if (failedAttempts >= 5)
            {
                ModelState.AddModelError(string.Empty, "Your account is temporarily locked due to multiple failed login attempts.");
                return View(model);
            }

            // Check if the password matches
            if (user.Password != EncryptionUtils.EncodeSha256(model.LoginPassword))
            {
                HttpContext.Session.SetInt32($"FailedAttempts_{model.LoginUserName}", failedAttempts + 1);
                if (failedAttempts + 1 >= 5)
                {
                    user.Status = 3; // Lock account after 5 failed attempts
                    await context.SaveChangesAsync();

                    ModelState.AddModelError(string.Empty, "Your account has been banned due to too many failed login attempts.");
                }
                else
                {
                    ModelState.AddModelError(string.Empty, "Invalid email/username or password.");
                }
                return View(model);
            }

            HttpContext.Session.SetInt32($"FailedAttempts_{model.LoginUserName}", 0); // Reset failed attempts after successful login

            // If account is pending verification
            if (user.Status == 2)
            {
                string token = GenerateVerificationToken(model.LoginUserName);
                string verificationLink = $"http://localhost:5222/Account/VerifyEmail?token={token}";
                string subject = "Please Verify Your Email Address";
                string body = $"Dear {model.LoginUserName},\n\nPlease click the following link to verify your email address: {verificationLink}";
                await emailUtils.SendEmailAsync(model.LoginUserName, subject, body);
                return RedirectToAction("Verify");
            }

            // If the account is banned
            if (user.Status == 3)
            {
                ModelState.AddModelError(string.Empty, "Your account has been banned. Please contact admin.");
                return View(model);
            }

            // Check user roles (seller or customer)
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

            string avatarUrl = "/images/user-avatar/default-avatar.jpg";

            if (userInfo != null)
            {
                avatarUrl = userInfo.AvatarUrl ?? avatarUrl;
            }

            HttpContext.Session.SetString("AvatarUrl", avatarUrl);
            HttpContext.Session.SetString("UserId", user.Id.ToString());
            HttpContext.Session.SetString("UserRole", model.UserType.ToUpper());

            // Redirect based on user type
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
            var adminUser = await context.Users.FirstOrDefaultAsync(u => u.Email == model.LoginUserName && u.Password == model.LoginPassword);
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
                Password = EncryptionUtils.EncodeSha256(model.Password),
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
                AvatarUrl = "/images/user-avatar/default-avatar.jpg",
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

                // Check if the user is already verified
                if (user.Status == 1)
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


        [HttpGet]
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





        //Settings for User
        public IActionResult UserProfile()
        {
            var userId = HttpContext.Session.GetString("UserId");
            if (userId == null) return RedirectToAction("Login");

            var user = context.Users.Include(u => u.Userinfo).FirstOrDefault(u => u.Id.ToString() == userId);
            if (user == null) return RedirectToAction("Login");

            UserViewModel vm = new UserViewModel()
            {
                AvatarUrl = user.Userinfo.AvatarUrl,
                UserName = user.UserName,
                Email = user.Email,
                FullName = user.Userinfo.FullName,
                PhoneNumber = user.Userinfo.PhoneNumber,
                Gender = user.Userinfo.Gender ?? 1,
                DateOfBirth = user.Userinfo.DateOfBirth ?? new DateOnly(2000, 1, 1),
                Address = user.Userinfo.Address,
            };

            return View(vm);
        }

        [HttpPost]
        public async Task<IActionResult> UserProfile(UserViewModel model)
        {
            if (!ModelState.IsValid)
            {
                return View(model);
            }

            var userId = HttpContext.Session.GetString("UserId");
            string validationError = ValidationUpdateUser(model, model.AvatarFile);
            if (validationError != null)
            {
                ModelState.AddModelError(string.Empty, validationError);
                return View(model);
            }

            // Check if username is unique
            var userExists = await context.Users.AnyAsync(u => u.UserName == model.UserName && u.Email != model.Email);
            if (userExists) ModelState.AddModelError(string.Empty, "Username already exists!");

            // Check if phone number is unique
            var phoneExists = await context.Users.AnyAsync(u => u.Userinfo.PhoneNumber == model.PhoneNumber && u.Email != model.Email);
            if (phoneExists) ModelState.AddModelError(string.Empty, "Phone number already exists!");

            User user = await context.Users.FirstOrDefaultAsync(u => u.Id.ToString() == userId);
            if (user != null)
            {
                user.UserName = model.UserName;
                context.Update(user);
                await context.SaveChangesAsync();
            }

            Userinfo userInfo = await context.Userinfos.FirstOrDefaultAsync(u => u.Id.ToString() == userId);
            if (userInfo != null)
            {
                userInfo.FullName = model.FullName;
                userInfo.PhoneNumber = model.PhoneNumber;

                // Handle avatar file if it is provided
                if (model.AvatarFile != null && model.AvatarFile.Length > 0)
                {
                    string fileName = $"{model.UserName}-avatar.jpg"; // Generate a unique file name
                    string filePath = Path.Combine(Directory.GetCurrentDirectory(), "wwwroot", "images", "user-avatar", fileName);

                    // Save the avatar image to the file system
                    using (var stream = new FileStream(filePath, FileMode.Create))
                    {
                        await model.AvatarFile.CopyToAsync(stream);
                    }

                    // Set the AvatarUrl if file is uploaded
                    userInfo.AvatarUrl = $"/images/user-avatar/{fileName}";
                    HttpContext.Session.SetString("AvatarUrl", $"/images/user-avatar/{fileName}");
                }

                // Update other fields
                userInfo.Gender = model.Gender;
                userInfo.DateOfBirth = model.DateOfBirth;
                userInfo.Address = model.Address;
                userInfo.UpdateAt = DateTime.UtcNow;

                context.Update(userInfo);
                await context.SaveChangesAsync();
            }

            return RedirectToAction("UserProfile");
        }


        public string ValidationUpdateUser(UserViewModel model, IFormFile avatarFile)
        {
            if (!ValidationUtils.IsValidUserName(model.UserName)) return "Please enter valid username! Only alphabet, number and underscore.";
            if (!ValidationUtils.IsValidMobile(model.PhoneNumber)) return "Please enter valid phone number.";
            if (avatarFile != null)
            {
                if (!ValidationUtils.IsValidAvatar(avatarFile)) return "Only accept image file jpg, jpeg, png, gif";
            }
            if (!ValidationUtils.IsValidGender(gender: model.Gender)) return "Please choose a gender.";
            if (!ValidationUtils.IsValidDateOfBirth(model.DateOfBirth)) return "You must be older than 18 years old";
            return null;
        }
    }
}
