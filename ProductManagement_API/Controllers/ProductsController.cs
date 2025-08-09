using Microsoft.AspNetCore.Mvc;

namespace ProductManagement_API.Controllers
{
    public class ProductsController : Controller
    {
        public IActionResult Index()
        {
            return View();
        }
    }
}
