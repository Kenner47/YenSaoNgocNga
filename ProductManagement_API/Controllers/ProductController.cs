using Microsoft.AspNetCore.Mvc;

namespace ProductManagement_API.Controllers
{
    public class ProductController : Controller
    {
        public IActionResult Index()
        {
            return View();
        }
    }
}
