using Microsoft.AspNetCore.Mvc;

namespace OnlineClothing.Controllers.Admin
{
    [Route("Admin/AdminChangePassword")]
    public class AdminChangePasswordController : Controller
    {
        public IActionResult Manage()
        {
            return View();
        }
    }
}
