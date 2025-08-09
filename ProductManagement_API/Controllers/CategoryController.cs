using Microsoft.AspNetCore.Mvc;

namespace ProductManagement_API.Controllers
{
    public class CategoryController : Controller
    {
        public IActionResult Index()
        {
            return View();
        }
    }
}
