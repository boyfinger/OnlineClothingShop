using Microsoft.AspNetCore.Mvc;

namespace OnlineClothing.Controllers
{
    public class ErrorController : Controller
    {
        [Route("Error/{statusCode}")]
        public IActionResult HandleError(int statusCode)
        {
            ViewData["StatusCode"] = statusCode;

            if (statusCode == 404)
            {
                ViewData["ErrorMessage"] = "Không tìm thấy trang hoặc sản phẩm.";
            }
            else
            {
                ViewData["ErrorMessage"] = "Đã có lỗi xảy ra.";
            }

            return View("Error");
        }
    }
}
