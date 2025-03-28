﻿using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using OnlineClothing.Models;
using System;
using System.Linq;
using System.Threading.Tasks;

namespace OnlineClothing.Controllers.Admin
{
    [Route("Admin/Customer")]
    public class CustomerController : Controller
    {
        private readonly ClothingShopPrn222G2Context _context;
        private readonly IWebHostEnvironment _webHostEnvironment;
        public CustomerController(ClothingShopPrn222G2Context context, IWebHostEnvironment webHostEnvironment)
        {
            _context = context;
            _webHostEnvironment = webHostEnvironment;
        }
        // GET: Admin/Customer/Manage
        [HttpGet("Manage")]
        public async Task<IActionResult> Manage(string search, int? status, int page = 1)
        {
            int pageSize = 10;
            var query = _context.Users
                .Include(u => u.UserRoles)
                .ThenInclude(ur => ur.Role)
                .Where(u => u.UserRoles.Any(ur => ur.RoleId == 3));
            // tim kiem theo user name or email
            if (!string.IsNullOrEmpty(search))
            {
                // Tách các từ khóa tìm kiếm
                var searchTerms = search.Split(new[] { ' ' }, StringSplitOptions.RemoveEmptyEntries);

                foreach (var term in searchTerms)
                {
                    var lowerTerm = term.ToLower();
                    query = query.Where(u =>
                        u.UserName.ToLower().Contains(lowerTerm) ||
                        u.Email.ToLower().Contains(lowerTerm));
                }
            }            // loc theo status
            if (status.HasValue)
            {
                query = query.Where(u => u.Status == status.Value);
            }
            // phan trang
            var totalRecords = await query.CountAsync();
            var users = await query.OrderBy(u => u.CreatedAt)
                .Skip((page - 1) * pageSize)
                .Take(pageSize)
                .ToListAsync();
            ViewBag.TotalPages = (int)Math.Ceiling(totalRecords / (double)pageSize);
            ViewBag.CurrentPage = page;
            ViewBag.Search = search;
            ViewBag.Status = status;
            return View(users);
        }
        // GET: Admin/Customer/Details/{id}
        [HttpGet("Details/{id}")]
        public async Task<IActionResult> Details(Guid id)
        {
            var user = await _context.Users
                .Include(u => u.Userinfo)
                .ThenInclude(ui => ui.GenderNavigation)
                .FirstOrDefaultAsync(u => u.Id == id);

            if (user == null) return NotFound();

            var userDetails = new
            {
                userInfo = new
                {
                    fullName = user.Userinfo?.FullName,
                    phoneNumber = user.Userinfo?.PhoneNumber,
                    avatarUrl = user.Userinfo?.AvatarUrl,
                    gender = user.Userinfo?.GenderNavigation?.Name,
                    dateOfBirth = user.Userinfo?.DateOfBirth?.ToString("yyyy-MM-dd"),
                    address = user.Userinfo?.Address,
                    updatedAt = user.Userinfo?.UpdateAt?.ToString("yyyy-MM-dd HH:mm:ss")
                }
            };

            return Json(userDetails);
        }

        // GET: Admin/Customer/Add
        [HttpGet("Add")]
        public IActionResult Add()
        {
            var genders = _context.UserGenders.ToList();
            ViewBag.Genders = new SelectList(genders, "Id", "Name");

            return PartialView("_AddCustomerPartial");
        }
        [HttpPost("Add")]
        public async Task<IActionResult> Add(AdminUserViewModel model)
        {
            if (ModelState.IsValid)
            {
                try
                {
                    var user = new User
                    {
                        Id = Guid.NewGuid(),
                        UserName = model.UserName,
                        Email = model.Email,
                        Password = "123456", 
                        Status = 1, 
                        CreatedAt = DateTime.Now
                    };

                    _context.Users.Add(user);


                    var userInfo = new Userinfo
                    {
                        Id = user.Id, 
                        FullName = model.FullName,
                        PhoneNumber = model.PhoneNumber,
                        AvatarUrl = model.AvatarUrl,
                        Gender = model.Gender,
                        DateOfBirth = model.DateOfBirth,
                        Address = model.Address,
                        UpdateAt = DateTime.Now,
                    };

                    _context.Userinfos.Add(userInfo);
                    var userRole = new UserRole
                    {
                        UserId = user.Id,
                        RoleId = 3 
                    };
                    _context.UserRoles.Add(userRole);
                    await _context.SaveChangesAsync();

                    return Json(new { success = true, userId = user.Id, message = "Thêm khách hàng thành công !" });
                }
                catch (Exception ex)
                {
                    return Json(new { success = false, message = "Đã xảy ra lỗi khi thêm người dùng: " + ex.Message });
                }
            }
            var errors = ModelState.Values
                .SelectMany(v => v.Errors)
                .Select(e => e.ErrorMessage)
                .ToList();

            return Json(new { success = false, message = "Dữ liệu không hợp lệ.", errors = errors });
        }
        // GET: Admin/Customer/Edit/{id}
        [HttpGet("Edit/{id}")]
        public async Task<IActionResult> Edit(Guid id)
        {
            var user = await _context.Users
                .Include(u => u.Userinfo)
                .FirstOrDefaultAsync(u => u.Id == id);

            if (user == null)
            {
                return NotFound();
            }

            var model = new AdminUserViewModel
            {
                Id = user.Id, 
                UserName = user.UserName,
                Email = user.Email,
                FullName = user.Userinfo?.FullName,
                PhoneNumber = user.Userinfo?.PhoneNumber,
                AvatarUrl = user.Userinfo?.AvatarUrl,
                Gender = user.Userinfo?.Gender,
                DateOfBirth = user.Userinfo?.DateOfBirth ?? DateOnly.MinValue,
                Address = user.Userinfo?.Address
            };

            var genders = await _context.UserGenders.ToListAsync();
            ViewBag.Genders = new SelectList(genders, "Id", "Name", user.Userinfo?.Gender);

            return PartialView("_EditCustomerPartial", model);
        }
        [HttpPost("Edit/{id}")]
        public async Task<IActionResult> Edit(Guid id, AdminUserViewModel model, IFormFile avatarFile)
        {
            if (ModelState.IsValid)
            {
                if (id != model.Id)
                {
                    return Json(new { success = false, message = "Id không khớp." });
                }

                var user = await _context.Users
                    .Include(u => u.Userinfo)
                    .FirstOrDefaultAsync(u => u.Id == id);

                if (user == null)
                {
                    return Json(new { success = false, message = "Không tìm thấy khách hàng." });
                }

                user.UserName = model.UserName;
                user.Email = model.Email;

                if (user.Userinfo == null)
                {
                    user.Userinfo = new Userinfo
                    {
                        Id = user.Id,
                        IdNavigation = user
                    };
                }

                // Xử lý upload ảnh đại diện
                if (avatarFile != null && avatarFile.Length > 0)
                {
                    // Kiểm tra định dạng file
                    var allowedExtensions = new[] { ".jpg", ".jpeg", ".png", ".gif" };
                    var fileExtension = Path.GetExtension(avatarFile.FileName).ToLowerInvariant();

                    if (!allowedExtensions.Contains(fileExtension))
                    {
                        return Json(new
                        {
                            success = false,
                            message = "Chỉ chấp nhận file ảnh có định dạng JPG, JPEG, PNG hoặc GIF."
                        });
                    }

                    // Kiểm tra kích thước file (tối đa 5MB)
                    if (avatarFile.Length > 5 * 1024 * 1024)
                    {
                        return Json(new
                        {
                            success = false,
                            message = "Kích thước file quá lớn (tối đa 5MB)."
                        });
                    }

                    // Tạo tên file duy nhất
                    var uniqueFileName = Guid.NewGuid().ToString() + fileExtension;
                    var uploadsFolder = Path.Combine(_webHostEnvironment.WebRootPath, "uploads/avatars");

                    // Đảm bảo thư mục tồn tại
                    if (!Directory.Exists(uploadsFolder))
                    {
                        Directory.CreateDirectory(uploadsFolder);
                    }

                    // Xóa ảnh cũ nếu có
                    if (!string.IsNullOrEmpty(user.Userinfo.AvatarUrl))
                    {
                        var oldFilePath = Path.Combine(_webHostEnvironment.WebRootPath, user.Userinfo.AvatarUrl.TrimStart('/'));
                        if (System.IO.File.Exists(oldFilePath))
                        {
                            System.IO.File.Delete(oldFilePath);
                        }
                    }

                    // Lưu file mới
                    var filePath = Path.Combine(uploadsFolder, uniqueFileName);
                    using (var fileStream = new FileStream(filePath, FileMode.Create))
                    {
                        await avatarFile.CopyToAsync(fileStream);
                    }

                    // Cập nhật đường dẫn ảnh mới
                    user.Userinfo.AvatarUrl = $"/uploads/avatars/{uniqueFileName}";
                }
                else if (!string.IsNullOrEmpty(model.AvatarUrl))
                {
                    // Giữ nguyên ảnh cũ nếu không có file upload và có AvatarUrl từ model
                    user.Userinfo.AvatarUrl = model.AvatarUrl;
                }

                user.Userinfo.FullName = model.FullName;
                user.Userinfo.PhoneNumber = model.PhoneNumber;
                user.Userinfo.Gender = model.Gender;
                user.Userinfo.DateOfBirth = model.DateOfBirth;
                user.Userinfo.Address = model.Address;
                user.Userinfo.UpdateAt = DateTime.Now;

                try
                {
                    await _context.SaveChangesAsync();
                    return Json(new
                    {
                        success = true,
                        message = "Chỉnh sửa thông tin khách hàng thành công!",
                        avatarUrl = user.Userinfo.AvatarUrl // Trả về đường dẫn ảnh mới nếu cần
                    });
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!UserExists(id))
                    {
                        return Json(new { success = false, message = "Không tìm thấy khách hàng." });
                    }
                    else
                    {
                        throw;
                    }
                }
                catch (Exception ex)
                {
                    return Json(new
                    {
                        success = false,
                        message = "Đã xảy ra lỗi khi lưu thay đổi: " + ex.Message
                    });
                }
            }

            // Nếu ModelState không hợp lệ, trả về thông báo lỗi chi tiết
            var errors = ModelState.Values
                .SelectMany(v => v.Errors)
                .Select(e => e.ErrorMessage)
                .ToList();

            return Json(new
            {
                success = false,
                message = "Dữ liệu không hợp lệ.",
                errors = errors
            });
        }

        private bool UserExists(Guid id)
        {
            return _context.Users.Any(e => e.Id == id);
        }


        // POST: Admin/Customer/BlockUnblock/{id}
        [HttpPost("BlockUnblock/{id}")]
        public async Task<IActionResult> BlockUnblock(Guid id)
        {
            var user = await _context.Users.FindAsync(id);
            if (user == null) return NotFound();

            user.Status = user.Status == 1 ? 3 : 1; 
            await _context.SaveChangesAsync();

            return Json(new { success = true, message = "User status updated successfully!" });
        }
    }
}
