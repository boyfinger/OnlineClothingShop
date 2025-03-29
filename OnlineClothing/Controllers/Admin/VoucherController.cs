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
            var now = DateTime.Now;

            var expiredVouchers = await _context.Vouchers
                .Where(v => v.EndDate <= now && v.Status == 1)
                .ToListAsync();

            if (expiredVouchers.Any())
            {
                foreach (var voucher in expiredVouchers)
                {
                    voucher.Status = 2; 
                }
                await _context.SaveChangesAsync();
            }


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

            var vouchers = await query.ToListAsync();

            var statusOrder = new List<int?> { 5, 1, 3, 2, 4 };

            vouchers = vouchers
                .OrderBy(v => statusOrder.IndexOf(v.Status))
                .ThenByDescending(v => v.CreateAt)
                .Skip((page - 1) * pageSize)
                .Take(pageSize)
                .ToList();

            var totalVouchers = await _context.Vouchers.CountAsync();
            var activeVouchers = await _context.Vouchers.CountAsync(v => v.Status == 1);
            var expiringVouchers = await _context.Vouchers.CountAsync(v => v.EndDate <= now.AddDays(7) && v.EndDate > now);
            var expiredVouchersCount = await _context.Vouchers.CountAsync(v => v.Status == 2);

            ViewBag.TotalVouchers = totalVouchers;
            ViewBag.ActiveVouchers = activeVouchers;
            ViewBag.ExpiringVouchers = expiringVouchers;
            ViewBag.ExpiredVouchers = expiredVouchersCount;
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
            try
            {
                // Validate ModelState
                if (!ModelState.IsValid)
                {
                    var errors = ModelState.Values
                        .SelectMany(v => v.Errors)
                        .Select(e => e.ErrorMessage)
                        .ToList();
                    return Json(new { success = false, message = "Dữ liệu không hợp lệ", errors = errors });
                }

                if (voucher.StartDate >= voucher.EndDate)
                {
                    return Json(new { success = false, message = "Ngày bắt đầu phải trước ngày kết thúc" });
                }

                if (await _context.Vouchers.AnyAsync(v => v.Code == voucher.Code))
                {
                    return Json(new { success = false, message = "Mã voucher đã tồn tại" });
                }
                if (voucher.Value <= 0)
                {
                    return Json(new { success = false, message = "Giá trị voucher phải lớn hơn 0" });
                }

                if (voucher.UsageLimit <= 0)
                {
                    return Json(new { success = false, message = "Số lần sử dụng phải lớn hơn 0" });
                }
                if (voucher.StartDate < DateTime.Now.Date)
                {
                    return Json(new { success = false, message = "Ngày bắt đầu không được trong quá khứ" });
                }

                voucher.Status = 5; 
                voucher.CreateAt = DateTime.Now;

                _context.Vouchers.Add(voucher);
                await _context.SaveChangesAsync();

                return Json(new { success = true, message = "Thêm voucher thành công!" });
            }
            catch (Exception ex)
            {
                return Json(new { success = false, message = $"Lỗi hệ thống: {ex.Message}" });
            }
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
            try
            {
                // Validate ModelState
                if (!ModelState.IsValid)
                {
                    var errors = ModelState.Values
                        .SelectMany(v => v.Errors)
                        .Select(e => e.ErrorMessage)
                        .ToList();
                    return Json(new { success = false, message = "Dữ liệu không hợp lệ", errors = errors });
                }
                if (id != voucher.Id)
                {
                    return Json(new { success = false, message = "ID voucher không khớp" });
                }

                if (voucher.StartDate >= voucher.EndDate)
                {
                    return Json(new { success = false, message = "Ngày bắt đầu phải trước ngày kết thúc" });
                }

                var existingVoucher = await _context.Vouchers.FindAsync(id);
                if (existingVoucher == null)
                {
                    return Json(new { success = false, message = "Không tìm thấy voucher" });
                }

                if (await _context.Vouchers.AnyAsync(v => v.Code == voucher.Code && v.Id != id))
                {
                    return Json(new { success = false, message = "Mã voucher đã tồn tại" });
                }

                // Validate giá trị voucher
                if (voucher.Value <= 0)
                {
                    return Json(new { success = false, message = "Giá trị voucher phải lớn hơn 0" });
                }

                // Validate số lần sử dụng
                if (voucher.UsageLimit <= 0)
                {
                    return Json(new { success = false, message = "Số lần sử dụng phải lớn hơn 0" });
                }

                // Validate ngày bắt đầu không được trong quá khứ (nếu thay đổi)
                if (voucher.StartDate != existingVoucher.StartDate && voucher.StartDate < DateTime.Now.Date)
                {
                    return Json(new { success = false, message = "Ngày bắt đầu không được trong quá khứ" });
                }

                // Cập nhật thông tin
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

                return Json(new { success = true, message = "Cập nhật voucher thành công!" });
            }
            catch (Exception ex)
            {
                return Json(new { success = false, message = $"Lỗi hệ thống: {ex.Message}" });
            }
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
