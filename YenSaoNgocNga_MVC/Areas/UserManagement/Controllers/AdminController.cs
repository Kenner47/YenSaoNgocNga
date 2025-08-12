using Microsoft.AspNetCore.Mvc;

namespace YenSaoNgocNga_MVC.Areas.UserManagement.Controllers
{
    [Area("UserManagement")]
    public class AdminController : Controller
    {
        // GET: /UserManagement/Admin/Dashboard
        public IActionResult Dashboard()
        {
            // Kiểm tra quyền Admin
            var userRole = HttpContext.Session.GetString("UserRole");
            if (string.IsNullOrEmpty(userRole) ||
                (!userRole.Equals("Admin", StringComparison.OrdinalIgnoreCase) &&
                 !userRole.Equals("SuperAdmin", StringComparison.OrdinalIgnoreCase)))
            {
                TempData["Error"] = "Bạn không có quyền truy cập trang này!";
                return RedirectToAction("Login", "Auth");
            }

            // Lấy thông tin user từ session
            ViewBag.UserName = HttpContext.Session.GetString("UserName");
            ViewBag.UserEmail = HttpContext.Session.GetString("UserEmail");
            ViewBag.UserRole = HttpContext.Session.GetString("UserRole");

            return View();
        }

        // GET: /UserManagement/Admin/Users
        public IActionResult Users()
        {
            if (!IsAdmin()) return RedirectToAction("Login", "Auth");
            return View();
        }

        // GET: /UserManagement/Admin/Products
        public IActionResult Products()
        {
            if (!IsAdmin()) return RedirectToAction("Login", "Auth");
            return View();
        }

        // GET: /UserManagement/Admin/Orders
        public IActionResult Orders()
        {
            if (!IsAdmin()) return RedirectToAction("Login", "Auth");
            return View();
        }

        // GET: /UserManagement/Admin/Reports
        public IActionResult Reports()
        {
            if (!IsAdmin()) return RedirectToAction("Login", "Auth");
            return View();
        }

        // GET: /UserManagement/Admin/Settings
        public IActionResult Settings()
        {
            if (!IsAdmin()) return RedirectToAction("Login", "Auth");
            return View();
        }

        // Helper method to check admin role
        private bool IsAdmin()
        {
            var userRole = HttpContext.Session.GetString("UserRole");
            return !string.IsNullOrEmpty(userRole) &&
                   (userRole.Equals("Admin", StringComparison.OrdinalIgnoreCase) ||
                    userRole.Equals("SuperAdmin", StringComparison.OrdinalIgnoreCase));
        }
    }
}