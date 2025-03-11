using Microsoft.AspNetCore.Mvc;
using OnlineClothing.Services;

namespace OnlineClothing.Controllers
{
    public class ApiController : Controller
    {
        private readonly IOpenAIService _service;

        public ApiController(IOpenAIService service)
        {
            _service = service;
        }

        public string CheckDescription(string description)
        {
            return _service.CheckDescription(description).Content[0].Text;
        }
    }
}
