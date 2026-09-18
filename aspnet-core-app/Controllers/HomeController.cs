using Microsoft.AspNetCore.Mvc;

namespace aspnet_core_app.Controllers
{
    public class HomeController : Controller
    {
        public IActionResult Index()
        {
            return View();
        }

        public IActionResult Products()
        {
            return View();
        }

        public IActionResult Users()
        {
            return View();
        }
    }
}