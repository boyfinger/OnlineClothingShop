using Microsoft.AspNetCore.Mvc;


namespace OnlineClothing.Controllers
{
    public class AccountController : Controller
    {
        public IActionResult Login()
        {
            return View();
        }
    }
}
