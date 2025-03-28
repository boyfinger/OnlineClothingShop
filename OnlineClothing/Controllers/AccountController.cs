﻿using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using OnlineClothing.Models;
using OnlineClothing.Services;
using OnlineClothing.Utils;
using System.Text;

namespace OnlineClothing.Controllers
{
    public class AccountController : Controller
    {
        private readonly ClothingShopPrn222G2Context _context;
        private readonly IEmailService _emailService;
        private readonly ILogger<AccountController> _logger;
        private readonly CloudinaryService _cloudinaryService;

        private static readonly string _defaultAvatarImage = "https://res.cloudinary.com/dvyswwdcz/image/upload/v1743173655/uhvl2gm1jnnevuvrspdh.jpg";

        public AccountController(ClothingShopPrn222G2Context context, IEmailService emailService, ILogger<AccountController> logger, CloudinaryService cloudinaryService)
        {
            this._context = context;
            this._emailService = emailService;
            this._logger = logger;
            this._cloudinaryService = cloudinaryService;
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
                    return RedirectToAction("Dashboard", "SellerProducts");
                }
                else if (userRole == "CUSTOMER")
                {
                    return RedirectToAction("Index", "Home");
                }
                else if (userRole == "ADMIN")
                {
                    return RedirectToAction("Dashboard", "AdminDashboard");
                }
            }
            return View();
        }


        //Handle Login action
        public async Task<IActionResult> Login(LoginViewModel model)
        {
            if (!ModelState.IsValid)
            {
                return View(model);
            }

            // Check if user exists using either email or username
            var user = await _context.Users
                                    .FirstOrDefaultAsync(u => u.Email == model.LoginUserName || u.UserName == model.LoginUserName);

            // Check if user is found in the database
            if (user == null)
            {
                ModelState.AddModelError(string.Empty, "Invalid email/username or password.");
                return View(model);
            }

            //Check if user login failed
            int failedAttempts = HttpContext.Session.GetInt32($"FailedAttempts_{model.LoginUserName}") ?? 0;
            if (failedAttempts >= 5)
            {
                ModelState.AddModelError(string.Empty, "Your account is temporarily locked due to multiple failed login attempts.");
                return View(model);
            }

            // Check if the password matches
            if (user.Password != EncryptionUtils.EncodeSha256(model.LoginPassword))
            {
                //If user login failed more than 5 times, banned account
                HttpContext.Session.SetInt32($"FailedAttempts_{model.LoginUserName}", failedAttempts + 1);
                if (failedAttempts + 1 >= 5)
                {
                    user.Status = 3;
                    await _context.SaveChangesAsync();

                    ModelState.AddModelError(string.Empty, "Your account has been banned due to too many failed login attempts.");
                }
                else
                {
                    ModelState.AddModelError(string.Empty, "Invalid email/username or password.");
                }
                return View(model);
            }

            HttpContext.Session.SetInt32($"FailedAttempts_{model.LoginUserName}", 0);

            // If account is pending verification
            if (user.Status == 2)
            {
                string token = GenerateVerificationToken(model.LoginUserName);
                string verificationLink = $"http://localhost:5222/Account/VerifyEmail?token={token}";
                string subject = "Please Verify Your Email Address";
                string body = $"Dear {model.LoginUserName},\n\nPlease click the following link to verify your email address: {verificationLink}";
                await _emailService.SendEmailAsync(model.LoginUserName, subject, body);
                return RedirectToAction("Verify");
            }

            // If the account is banned
            if (user.Status == 3)
            {
                ModelState.AddModelError(string.Empty, "Your account has been banned. Please contact admin.");
                return View(model);
            }

            // Check user roles (seller or customer)
            var userRoles = await _context.UserRoles
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

            var userInfo = await _context.Userinfos.FirstOrDefaultAsync(ui => ui.Id == user.Id);
            string avatarUrl = _defaultAvatarImage;
            if (userInfo != null)
            {
                avatarUrl = userInfo.AvatarUrl ?? avatarUrl;
            }

            HttpContext.Session.SetString("AvatarUrl", avatarUrl);
            HttpContext.Session.SetString("UserId", user.Id.ToString());
            HttpContext.Session.SetString("UserRole", model.UserType.ToUpper());

            if (isCustomer)
            {
                return RedirectToAction("Index", "Home");
            }
            else
            {
                return RedirectToAction("Dashboard", "SellerProducts");
            }
        }

        [HttpGet]
        public IActionResult AdminLogin()
        {
            var userId = HttpContext.Session.GetString("UserId");
            var userRole = HttpContext.Session.GetString("UserRole");

            if (userId != null && userRole == "ADMIN")
            {
                return RedirectToAction("Dashboard", "AdminDashboard");
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
            var adminUser = await _context.Users
                .FirstOrDefaultAsync(u => u.Email == model.LoginUserName && u.Password == EncryptionUtils.EncodeSha256(model.LoginPassword));
            if (adminUser == null)
            {
                ModelState.AddModelError(string.Empty, "Wrong email or password.");
                return View(model);
            }

            var isAdmin = await _context.UserRoles
                .AnyAsync(ur => ur.UserId == adminUser.Id && ur.RoleId == 1);

            if (!isAdmin)
            {
                ModelState.AddModelError(string.Empty, "You are not an admin.");
                return View(model);
            }

            HttpContext.Session.SetString("UserId", adminUser.Id.ToString());
            HttpContext.Session.SetString("UserRole", "ADMIN");

            return RedirectToAction("Dashboard", "AdminDashboard");
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
            var existingUserByEmail = await _context.Users
                .FirstOrDefaultAsync(u => u.Email == model.Email);
            if (existingUserByEmail != null)
            {
                ModelState.AddModelError(string.Empty, "An account with this email already exists.");
                return View(model);
            }
            var existingUserByUsername = await _context.Users
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
            _context.Users.Add(newUser);
            await _context.SaveChangesAsync();
            var userInfo = new Userinfo
            {
                Id = newUser.Id,
                FullName = model.FullName,
                PhoneNumber = model.PhoneNumber,
                AvatarUrl = _defaultAvatarImage,
                Gender = model.Gender == "male" ? 1 : model.Gender == "female" ? 2 : 3,
                DateOfBirth = model.DateOfBirth,
                Address = model.Address,
                UpdateAt = DateTime.UtcNow
            };
            _context.Userinfos.Add(userInfo);
            await _context.SaveChangesAsync();
            if (model.UserType == "customer")
            {
                _context.UserRoles.Add(new UserRole
                {
                    UserId = newUser.Id,
                    RoleId = 3
                });
            }
            else if (model.UserType == "seller")
            {
                _context.UserRoles.Add(new UserRole
                {
                    UserId = newUser.Id,
                    RoleId = 2
                });
            }
            await _context.SaveChangesAsync();
            HttpContext.Session.SetString("UserRole", model.UserType.ToUpper());
            HttpContext.Session.SetString("UserId", newUser.Id.ToString());
            string token = GenerateVerificationToken(newUser.Email);
            string verificationLink = $"http://localhost:5222/Account/VerifyEmail?token={token}";
            string subject = "Please Verify Your Email Address";
            string body = $"Dear {model.FullName},\n\nPlease click the following link to verify your email address: {verificationLink}";
            await _emailService.SendEmailAsync(model.Email, subject, body);
            return RedirectToAction("Verify");
        }

        [HttpGet]
        public IActionResult VerifyEmail(string token)
        {
            try
            {
                var email = DecodeVerificationToken(token);
                var user = _context.Users.Include(u => u.Userinfo).FirstOrDefault(u => u.Email == email);

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
                HttpContext.Session.SetString("UserId", user.Id.ToString());
                _context.SaveChanges();
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
            HttpContext.Session.Remove("UserId");
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
        [HttpPost]
        public IActionResult LogoutAdmin()
        {
            HttpContext.Session.Clear();
            return RedirectToAction("AdminLogin", "Account");
        }

        [HttpGet]
        public IActionResult UserProfile()
        {
            var userId = HttpContext.Session.GetString("UserId");
            var avatarUrl = HttpContext.Session.GetString("AvatarUrl");
            if (avatarUrl == null && userId != null) return RedirectToAction("Logout");
            if (userId == null) return RedirectToAction("Login");

            var user = _context.Users.Include(u => u.Userinfo).FirstOrDefault(u => u.Id.ToString() == userId);
            if (user == null) return RedirectToAction("Login");

            UserViewModel vm = new UserViewModel()
            {
                AvatarUrl = user.Userinfo.AvatarUrl ?? "/images/user-avatar/default-avatar.jpg",
                UserName = user.UserName ?? "",
                Email = user.Email ?? "",
                FullName = user.Userinfo.FullName ?? "",
                PhoneNumber = user.Userinfo.PhoneNumber ?? "",
                Gender = user.Userinfo.Gender ?? 1,
                DateOfBirth = user.Userinfo.DateOfBirth ?? new DateOnly(2000, 1, 1),
                Address = user.Userinfo.Address ?? "",
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
            var userExists = await _context.Users.AnyAsync(u => u.UserName == model.UserName && u.Email != model.Email);
            if (userExists) ModelState.AddModelError(string.Empty, "Username already exists!");

            // Check if phone number is unique
            var phoneExists = await _context.Users.AnyAsync(u => u.Userinfo.PhoneNumber == model.PhoneNumber && u.Email != model.Email);
            if (phoneExists) ModelState.AddModelError(string.Empty, "Phone number already exists!");

            User user = await _context.Users.FirstOrDefaultAsync(u => u.Id.ToString() == userId);
            if (user != null)
            {
                user.UserName = model.UserName;
                _context.Update(user);
                await _context.SaveChangesAsync();
            }

            Userinfo userInfo = await _context.Userinfos.FirstOrDefaultAsync(u => u.Id.ToString() == userId);
            if (userInfo != null)
            {
                userInfo.FullName = model.FullName;
                userInfo.PhoneNumber = model.PhoneNumber;

                // Handle avatar upload to Cloudinary (only update if upload succeeds)
                if (model.AvatarFile != null && model.AvatarFile.Length > 0)
                {
                    try
                    {
                        string newAvatarUrl = await _cloudinaryService.UploadImageAsync(model.AvatarFile, 600, 600);
                        if (!string.IsNullOrEmpty(newAvatarUrl))
                        {
                            userInfo.AvatarUrl = newAvatarUrl; // Update only if upload succeeds
                            HttpContext.Session.SetString("AvatarUrl", newAvatarUrl);
                        }
                        // If upload fails, retain the existing avatar (no change)
                    }
                    catch (Exception ex)
                    {
                        _logger.LogError(ex, "Cloudinary upload failed for user avatar");
                        // Silently keep the old avatar
                    }
                }

                // Update other fields
                userInfo.Gender = model.Gender;
                userInfo.DateOfBirth = model.DateOfBirth;
                userInfo.Address = model.Address;
                userInfo.UpdateAt = DateTime.UtcNow;

                _context.Update(userInfo);
                await _context.SaveChangesAsync();
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

        public IActionResult ChangePassword()
        {
            var userId = HttpContext.Session.GetString("UserId");
            if (userId == null) return RedirectToAction("Index", "Home");

            return View();
        }

        [HttpPost]
        public async Task<IActionResult> ChangePassword(ChangePassViewModel model)
        {
            if (!ModelState.IsValid) return View(model);

            var userId = HttpContext.Session.GetString("UserId");
            var user = _context.Users.FirstOrDefault(u => u.Id.ToString() == userId);

            //compare old password
            string password_hash = EncryptionUtils.EncodeSha256(model.OldPassword);
            if (password_hash != user.Password.ToString())
            {
                ModelState.AddModelError(string.Empty, "Your current password is wrong.");
                return View(model);
            }
            if (model.OldPassword == model.NewPassword)
            {
                ModelState.AddModelError(string.Empty, "Your new password is the same as your old one.");
                return View(model);
            }
            if (model.NewPassword != model.ConfirmPassword)
            {
                ModelState.AddModelError(string.Empty, "You new password is not the same as the confirm one.");
                return View(model);
            }

            bool validationPassword = ValidationUtils.IsValidPassword(model.NewPassword);
            if (!validationPassword)
            {
                ModelState.AddModelError(string.Empty, "Password must have at least 8 characters long, at least 1 uppercase, at least 1 lowercase, at least 1 number");
                return View(model);
            }

            user.Password = EncryptionUtils.EncodeSha256(model.NewPassword);
            _context.Update(user);
            await _context.SaveChangesAsync();

            return RedirectToAction("ChangePasswordSuccess");
        }

        [HttpGet]
        public IActionResult ChangePasswordSuccess()
        {
            return View();
        }

        [HttpGet]
        public IActionResult ForgotPassword()
        {
            var userId = HttpContext.Session.GetString("UserId");
            if (userId != null)
            {
                var userRole = HttpContext.Session.GetString("UserRole");
                if (userRole == "SELLER")
                {
                    return RedirectToAction("Dashboard", "SellerProducts");
                }
                else if (userRole == "CUSTOMER")
                {
                    return RedirectToAction("Index", "Home");
                }
                else if (userRole == "ADMIN")
                {
                    return RedirectToAction("Dashboard", "AdminDashboard");
                }
            }
            return View();
        }

        [HttpPost]
        public async Task<IActionResult> ForgotPassword(ForgotModelView model)
        {
            if (!ModelState.IsValid)
            {
                return View(model);
            }

            //check if email is valid
            if (!ValidationUtils.IsValidEmail(model.Email))
            {
                ModelState.AddModelError(string.Empty, "Please enter valid email!");
                return View(model);
            }

            //check in database if the email is exists
            var user = await _context.Users
                .Where(u => u.Email == model.Email)
                .Select(u => new { u.Email, u.Userinfo.FullName })
                .FirstOrDefaultAsync();

            if (user == null)
            {
                return RedirectToAction("ForgotPasswordConfirmation");
            }

            //send reset email
            string token = GenerateResetPassToken(user.Email);

            string verificationLink = $"http://localhost:5222/Account/ResetPasswordCheck?token={token}";
            string subject = "Password Reset Link";
            string body = $"Dear {user.FullName},\n\nYou requested to reset your password. Click the link below: {verificationLink}\n\nThis link expires in 15 minutes.\n\n If you didn't request this, please ignore this email.";
            await _emailService.SendEmailAsync(model.Email, subject, body);
            return View("ForgotPassConfirm");
        }

        [HttpGet]
        public IActionResult ResetPasswordCheck(string token)
        {
            try
            {
                var email = DecodeResetToken(token);
                var user = _context.Users.Include(u => u.Userinfo).FirstOrDefault(u => u.Email == email);
                if (user == null)
                {
                    return View("NotFound");
                }

                var expirationDate = GetResetTokenExpirationDate(token);
                if (DateTime.UtcNow > expirationDate)
                {
                    return View("TokenExpired");
                }
                
                HttpContext.Session.SetString("ResetId", user.Id.ToString());
                return View("ResetPassword");
            }
            catch (Exception)
            {
                return View("ResetPassword");
            }
        }

        [HttpGet]
        public IActionResult ResetPassword(string token)
        {
            if (string.IsNullOrEmpty(token))
            {
                return View("NotFound");
            }
            var userId = HttpContext.Session.GetString("ResetId");

            if(userId == null)
            {
                return View("NotFound");
            }
            return View();
        }

        [HttpPost]
        public async Task<IActionResult> ResetPassword(ResetPasswordViewModel model)
        {
            if (!ModelState.IsValid) return View(model);

            var userId = HttpContext.Session.GetString("ResetId");
            var user = _context.Users.FirstOrDefault(u => u.Id.ToString() == userId);

            //check new password with confirm password
            if (model.NewPassword != model.ConfirmPassword)
            {
                ModelState.AddModelError(string.Empty, "You new password is not the same as the confirm one.");
                return View(model);
            }

            //validate password
            bool validationPassword = ValidationUtils.IsValidPassword(model.NewPassword);
            if (!validationPassword)
            {
                ModelState.AddModelError(string.Empty, "Password must have at least 8 characters long, at least 1 uppercase, 1 lowercase and 1 number");
                return View(model);
            }

            user.Password = EncryptionUtils.EncodeSha256(model.NewPassword);
            _context.Update(user);
            await _context.SaveChangesAsync();

            return View("ResetPasswordSuccess");
        }


        private string DecodeResetToken(string token)
        {
            var tokenData = Encoding.UTF8.GetString(Convert.FromBase64String(token));
            var email = tokenData.Split('&')[1];
            return email;
        }

        private DateTime GetResetTokenExpirationDate(string token)
        {
            var tokenData = Encoding.UTF8.GetString(Convert.FromBase64String(token));
            var expirationDate = DateTime.Parse(tokenData.Split('&')[2]);
            return expirationDate;
        }


        private string GenerateResetPassToken(string email)
        {
            string token = $"RESET&{email}&{DateTime.UtcNow.AddMinutes(15)}";
            return Convert.ToBase64String(Encoding.UTF8.GetBytes(token));
        }

    }
}
