using Microsoft.AspNetCore.Mvc;
using YenSaoNgocNga_MVC.Areas.UserManagement.Models;
using YenSaoNgocNga_MVC.Areas.UserManagement.Services;

namespace YenSaoNgocNga_MVC.Areas.UserManagement.Controllers
{
    [Area("UserManagement")]
    public class AuthController : Controller
    {
        private readonly IUserApiService _userApiService;
        private readonly ILogger<AuthController> _logger;

        public AuthController(IUserApiService userApiService, ILogger<AuthController> logger)
        {
            _userApiService = userApiService;
            _logger = logger;
        }

        // GET: /UserManagement/Auth/Login
        public IActionResult Login()
        {
            return View();
        }

        // POST: /UserManagement/Auth/Login
        [HttpPost]
        public async Task<IActionResult> Login(LoginViewModel model)
        {
            if (!ModelState.IsValid)
                return View(model);

            try
            {
                var result = await _userApiService.LoginAsync(model);

                if (result.IsSuccess)
                {
                    // Store user information in session
                    HttpContext.Session.SetString("AccessToken", result.AccessToken);
                    HttpContext.Session.SetString("UserName", result.UserName);
                    HttpContext.Session.SetString("UserEmail", result.Email);
                    HttpContext.Session.SetString("UserRole", result.RoleName);
                    HttpContext.Session.SetString("UserId", result.UserId.ToString());

                    TempData["Success"] = "Đăng nhập thành công!";

                    // Redirect dựa trên Role
                    if (result.RoleName.Equals("Admin", StringComparison.OrdinalIgnoreCase) ||
                        result.RoleName.Equals("SuperAdmin", StringComparison.OrdinalIgnoreCase))
                    {
                        return RedirectToAction("Dashboard", "Admin", new { area = "UserManagement" });
                    }
                    else
                    {
                        return RedirectToAction("Index", "Home", new { area = "" });
                    }
                }

                ModelState.AddModelError("", result.Message);
                return View(model);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Login error for user {Username}", model.Username);
                ModelState.AddModelError("", "Đã xảy ra lỗi: " + ex.Message);
                return View(model);
            }
        }

        // GET: /UserManagement/Auth/Register
        public IActionResult Register()
        {
            return View();
        }

        // POST: /UserManagement/Auth/Register
        [HttpPost]
        public async Task<IActionResult> Register(RegisterViewModel model)
        {
            if (!ModelState.IsValid)
                return View(model);

            try
            {
                var result = await _userApiService.RegisterAsync(model);

                if (result.IsSuccess)
                {
                    TempData["Success"] = "Đăng ký thành công! Vui lòng kiểm tra email để xác thực.";
                    return RedirectToAction("VerifyOtp", new { email = model.Email });
                }

                ModelState.AddModelError("", result.Message);
                return View(model);
            }
            catch (Exception ex)
            {
                ModelState.AddModelError("", "Đã xảy ra lỗi: " + ex.Message);
                return View(model);
            }
        }

        // GET: /UserManagement/Auth/VerifyOtp
        public IActionResult VerifyOtp(string email)
        {
            var model = new VerifyOtpViewModel { Email = email };
            return View(model);
        }

        // POST: /UserManagement/Auth/VerifyOtp
        [HttpPost]
        public async Task<IActionResult> VerifyOtp(VerifyOtpViewModel model)
        {
            if (!ModelState.IsValid)
                return View(model);

            try
            {
                var result = await _userApiService.VerifyOtpAsync(model);

                if (result.IsSuccess)
                {
                    TempData["Success"] = "Xác thực thành công! Bạn có thể đăng nhập.";
                    return RedirectToAction("Login");
                }

                ModelState.AddModelError("", result.Message);
                return View(model);
            }
            catch (Exception ex)
            {
                ModelState.AddModelError("", "Đã xảy ra lỗi: " + ex.Message);
                return View(model);
            }
        }

        // POST: /UserManagement/Auth/Logout
        [HttpPost]
        public IActionResult Logout()
        {
            HttpContext.Session.Clear();
            TempData["Success"] = "Đăng xuất thành công!";
            return RedirectToAction("Index", "Home", new { area = "" });
        }
    }
}