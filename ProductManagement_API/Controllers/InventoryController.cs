using Microsoft.AspNetCore.Mvc;

namespace ProductManagement_API.Controllers
{
    public class InventoryController : Controller
    {
        public IActionResult Index()
        {
            return View();
        }
    }
}
