using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using OnlineClothing.Models;

namespace OnlineClothing.Controllers.Admin
{
    [Route("Admin/Voucher")]
    public class VoucherController : Controller
    {
        private readonly ClothingShopPrn222G2Context _context;
        public VoucherController(ClothingShopPrn222G2Context context)
        {
            _context = context;
        }
        [HttpGet("Manage")]
        public async Task<IActionResult> Manage(string search, int? status, int? type, int page = 1)
        {
            int pageSize = 7;
            var query = _context.Vouchers.AsQueryable();
            if (!string.IsNullOrEmpty(search))
            {
                query = query.Where(v => v.Code.Contains(search));
            }
            if (status.HasValue)
            {
                query = query.Where(v => v.Status == status.Value);
            }
            if (type.HasValue)
            {
                query = query.Where(v => v.Type == type.Value);
            }

            var totalRecords = await query.CountAsync();
            var vouchers = await query.OrderBy(v => v.Id)
                .Skip((page - 1) * pageSize)
                .Take(pageSize)
                .ToListAsync();

            var totalVouchers = await _context.Vouchers.CountAsync();
            var activeVouchers = await _context.Vouchers.CountAsync(v => v.Status == 1);
            var expiringVouchers = await _context.Vouchers.CountAsync(v => v.EndDate <= DateTime.Now.AddDays(7) && v.EndDate > DateTime.Now);
            var expiredVouchers = await _context.Vouchers.CountAsync(v => v.EndDate <= DateTime.Now);

            ViewBag.TotalVouchers = totalVouchers;
            ViewBag.ActiveVouchers = activeVouchers;
            ViewBag.ExpiringVouchers = expiringVouchers;
            ViewBag.ExpiredVouchers = expiredVouchers;
            ViewBag.TotalPages = (int)Math.Ceiling(totalRecords / (double)pageSize);
            ViewBag.CurrentPage = page;
            ViewBag.Search = search;
            ViewBag.Status = status;
            ViewBag.Type = type;

            return View(vouchers);
        }
        public enum VoucherStatus
        {
            Active = 1,
            Expired = 2,
            Used = 3,
            Disabled = 4,
            Pending = 5
        }

        [HttpPost("ChangeStatus/{id}")]
        public async Task<IActionResult> ChangeStatus(long id)
        {
            try
            {
                var voucher = await _context.Vouchers.FindAsync(id);
                if (voucher == null)
                {
                    return Json(new { success = false, message = "Voucher không tồn tại." });
                }
                switch (voucher.Status)
                {
                    case 5:
                    case 4:
                        voucher.Status = 1;
                        break;
                    case 1:
                        voucher.Status = 4;
                        break;
                    default:
                        return Json(new { success = false, message = "Trạng thái không hợp lệ." });
                }

                _context.Vouchers.Update(voucher);
                await _context.SaveChangesAsync();
                return Json(new
                {
                    success = true,
                    message = "Cập nhật trạng thái thành công.",
                    newStatus = voucher.Status,
                    newStatusText = GetStatusText(voucher.Status)
                });
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Lỗi: {ex.Message}");
                return Json(new { success = false, message = "Đã xảy ra lỗi khi xử lý yêu cầu." });
            }
        }
        private string GetStatusText(int? status)
        {
            switch (status)
            {
                case 1: return "Đang hoạt động";
                case 2: return "Hết hạn";
                case 3: return "Đã hết lượt sử dụng";
                case 4: return "Vô hiệu hóa";
                case 5: return "Chờ xử lý";
                default: return "Không xác định";
            }
        }
        [HttpGet("Add")]
        public IActionResult Add()
        {
            return PartialView("_AddPartialView");
        }
        [HttpPost("Add")]
        public async Task<IActionResult> Add(Voucher voucher)
        {
            if (ModelState.IsValid)
            {
                voucher.Status = 5;
                voucher.CreateAt = DateTime.Now;


                _context.Vouchers.Add(voucher);
                await _context.SaveChangesAsync();

                return Json(new { success = true, message = "Voucher được thêm vào thành công!" });
            }

            return Json(new { success = false, message = "Invalid data." });
        }
        [HttpGet("Edit/{id}")]
        public async Task<IActionResult> Edit(long id)
        {
            var voucher = await _context.Vouchers.FindAsync(id); 
            if (voucher == null)
            {
                return NotFound();
            }
            return PartialView("_EditPartialView", voucher); 
        }
        [HttpPost("Edit/{id}")]
        public async Task<IActionResult> Edit(long id, Voucher voucher)
        {
            if (id != voucher.Id)
            {
                return NotFound();
            }

            if (ModelState.IsValid)
            {
                var existingVoucher = await _context.Vouchers.FindAsync(id);
                if (existingVoucher == null)
                {
                    return NotFound();
                }
                existingVoucher.Code = voucher.Code;
                existingVoucher.Type = voucher.Type;
                existingVoucher.Value = voucher.Value;
                existingVoucher.Description = voucher.Description;
                existingVoucher.StartDate = voucher.StartDate;
                existingVoucher.EndDate = voucher.EndDate;
                existingVoucher.UsageLimit = voucher.UsageLimit;
                existingVoucher.UpdateAt = DateTime.Now;

                _context.Vouchers.Update(existingVoucher);
                await _context.SaveChangesAsync();

                return Json(new { success = true, message = "Voucher đã chỉnh sửa thành công!" });
            }

            var errors = ModelState.Values
                .SelectMany(v => v.Errors)
                .Select(e => e.ErrorMessage)
                .ToList();

            return Json(new { success = false, message = "Dữ liệu không hợp lệ.", errors = errors });
        }
        [HttpPost("Delete/{id}")]
        public async Task<IActionResult> Delete(long id)
        {
            var voucher = await _context.Vouchers.FindAsync(id);
            if (voucher == null)
            {
                return Json(new { success = false, message = "Voucher không tồn tại." });
            }

            _context.Vouchers.Remove(voucher);
            await _context.SaveChangesAsync();

            return Json(new { success = true, message = "Voucher đã được xóa thành công!" });
        }

        [HttpGet("Info/{id}")]
        public async Task<IActionResult> Info(long id)
        {
            var voucherUsages = await _context.VoucherUsages
                .Include(vu => vu.User)
                .Where(vu => vu.VoucherId == id)
                .ToListAsync();

            return PartialView("_VoucherInfoPartial", voucherUsages);
        }


    }
}
