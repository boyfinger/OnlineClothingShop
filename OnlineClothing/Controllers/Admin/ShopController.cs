using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using OnlineClothing.Models;

namespace OnlineClothing.Controllers.Admin
{
    [Route("Admin/Shop")]
    public class ShopController : Controller
    {
        private readonly ClothingShopPrn222G2Context _context;
        public ShopController()
        {
            _context = new ClothingShopPrn222G2Context();
        }
        [HttpGet("Manage")]
        public async Task<IActionResult> Manage(string search, int? status, int page = 1)
        {
            int pageSize = 3;
            var query = _context.Users
                .Include(u => u.UserRoles)
                .ThenInclude(ur => ur.Role)
                .Include(u => u.Userinfo) 
                .Where(u => u.UserRoles.Any(ur => ur.RoleId == 2));

            if (status.HasValue)
            {
                query = query.Where(u => u.Status == status.Value);
            }

            var totalRecords = await query.CountAsync();
            var users = await query.OrderBy(u => u.Id)
                .Skip((page - 1) * pageSize)
                .Take(pageSize)
                .ToListAsync();

            var userViewModels = users.Select(u => new AdminUserViewModel
            {
                Id = u.Id,
                UserName = u.UserName ?? "N/A",
                Email = u.Email ?? "N/A",
                Status = u.Status,
                FullName = u.Userinfo?.FullName ?? "Không có tên",
                PhoneNumber = u.Userinfo?.PhoneNumber,
                AvatarUrl = u.Userinfo?.AvatarUrl,
                Gender = u.Userinfo?.Gender,
                DateOfBirth = (DateOnly?)(u.Userinfo?.DateOfBirth ?? null),
                Address = u.Userinfo?.Address ?? "Không có địa chỉ"
            }).ToList();

            ViewBag.TotalPages = (int)Math.Ceiling(totalRecords / (double)pageSize);
            ViewBag.CurrentPage = page;
            ViewBag.Search = search;
            ViewBag.Status = status;

            return View(userViewModels);
        }
        [HttpPost("ChangeStatus/{id}")]
        public async Task<IActionResult> ChangeStatus(Guid id, string action)
        {
            var user = await _context.Users
         .Include(u => u.Userinfo)
         .Include(u => u.UserRoles) 
         .FirstOrDefaultAsync(u => u.Id == id);

            if (user == null)
            {
                return Json(new { success = false, message = "Không tìm thấy cửa hàng." });
            }

            switch (action)
            {
                case "approve":
                    user.Status = 1; // Active
                    _context.Users.Update(user);
                    await _context.SaveChangesAsync();
                    return Json(new { success = true, message = "Cửa hàng đã được chấp thuận." });

                case "reject":
                    using (var transaction = await _context.Database.BeginTransactionAsync())
                    {
                        try
                        {
                            // Xóa tất cả UserRoles liên quan nếu có
                            if (user.UserRoles != null && user.UserRoles.Any())
                            {
                                _context.UserRoles.RemoveRange(user.UserRoles);
                                await _context.SaveChangesAsync();
                            }

                            // Xóa Userinfo trước nếu tồn tại
                            if (user.Userinfo != null)
                            {
                                _context.Userinfos.Remove(user.Userinfo);
                                await _context.SaveChangesAsync();
                            }

                            // Sau đó xóa User
                            _context.Users.Remove(user);
                            await _context.SaveChangesAsync();

                            // Commit Transaction
                            await transaction.CommitAsync();

                            return Json(new { success = true, message = "Yêu cầu của cửa hàng đã bị từ chối và xóa." });
                        }
                        catch (Exception ex)
                        {
                            await transaction.RollbackAsync();
                            return Json(new { success = false, message = "Lỗi khi xóa cửa hàng: " + ex.Message });
                        }
                    }
                case "ban":
                    user.Status = 3; // Banned
                    _context.Users.Update(user);
                    await _context.SaveChangesAsync();
                    return Json(new { success = true, message = "Cửa hàng đã bị cấm." });

                case "unban":
                    user.Status = 1; // Active
                    _context.Users.Update(user);
                    await _context.SaveChangesAsync();
                    return Json(new { success = true, message = "Cửa hàng đã được bỏ cấm." });

                default:
                    return Json(new { success = false, message = "Hành động không hợp lệ." });
            }
        }
    }
}
