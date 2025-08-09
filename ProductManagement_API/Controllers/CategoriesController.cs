using Microsoft.AspNetCore.Mvc;

namespace ProductManagement_API.Controllers
{
    public class CategoriesController : Controller
    {
        public IActionResult Index()
        {
            return View();
        }
    }
}
