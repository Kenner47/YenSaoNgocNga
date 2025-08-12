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
            _logger.LogInformation("🔥 LOGIN ATTEMPT: User {Username}", model?.Username ?? "NULL");
            _logger.LogInformation("🔥 ModelState.IsValid: {IsValid}", ModelState.IsValid);

            // Log all ModelState errors
            foreach (var key in ModelState.Keys)
            {
                var state = ModelState[key];
                _logger.LogInformation("🔥 ModelState[{Key}]: Value={Value}, Errors={ErrorCount}",
                    key, state?.AttemptedValue ?? "NULL", state?.Errors.Count ?? 0);

                if (state?.Errors.Count > 0)
                {
                    foreach (var error in state.Errors)
                    {
                        _logger.LogError("🔥 Error for {Key}: {ErrorMessage}", key, error.ErrorMessage);
                    }
                }
            }

            if (!ModelState.IsValid)
            {
                _logger.LogWarning("🔥 LOGIN FAILED: ModelState không hợp lệ");
                return View(model);
            }

            try
            {
                _logger.LogInformation("🔥 Gọi API login...");
                var result = await _userApiService.LoginAsync(model);

                _logger.LogInformation("🔥 API Response: Success={IsSuccess}, Role={RoleName}",
                    result.IsSuccess, result.RoleName);

                if (result.IsSuccess)
                {
                    // Store user information in session
                    HttpContext.Session.SetString("AccessToken", result.AccessToken);
                    HttpContext.Session.SetString("UserName", result.UserName);
                    HttpContext.Session.SetString("UserEmail", result.Email);
                    HttpContext.Session.SetString("UserRole", result.RoleName);
                    HttpContext.Session.SetString("UserId", result.UserId.ToString());

                    _logger.LogInformation("🔥 Session saved. Role: {Role}", result.RoleName);

                    TempData["Success"] = "Đăng nhập thành công!";

                    // 🔥 PHÂN QUYỀN: Redirect dựa trên Role
                    if (result.RoleName.Equals("Admin", StringComparison.OrdinalIgnoreCase) ||
                        result.RoleName.Equals("SuperAdmin", StringComparison.OrdinalIgnoreCase))
                    {
                        _logger.LogInformation("🔥 Redirecting to Admin Dashboard");
                        return RedirectToAction("Dashboard", "Admin", new { area = "UserManagement" });
                    }
                    else
                    {
                        _logger.LogInformation("🔥 Redirecting to Home for role: {Role}", result.RoleName);
                        return RedirectToAction("Index", "Home", new { area = "" });
                    }
                }

                _logger.LogWarning("🔥 LOGIN FAILED: {Message}", result.Message);
                ModelState.AddModelError("", result.Message);
                return View(model);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "🔥 LOGIN ERROR: {Message}", ex.Message);
                ModelState.AddModelError("", "Đã xảy ra lỗi: " + ex.Message);
                return View(model);
            }
        }

        // 🔥 TEST ACTION - Xóa sau khi fix xong
        [HttpPost]
        public async Task<IActionResult> TestLogin(string username, string password)
        {
            _logger.LogInformation("🔥 TEST LOGIN: username={Username}, password={PasswordLength}",
                username, password?.Length ?? 0);

            var model = new LoginViewModel
            {
                Username = username ?? "admin",
                Password = password ?? "admin123"
            };

            try
            {
                var result = await _userApiService.LoginAsync(model);
                _logger.LogInformation("🔥 TEST Result: {IsSuccess}, {Message}, Role: {Role}",
                    result.IsSuccess, result.Message, result.RoleName);

                return Json(new
                {
                    success = result.IsSuccess,
                    message = result.Message,
                    role = result.RoleName,
                    username = result.UserName
                });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "🔥 TEST ERROR");
                return Json(new { success = false, message = ex.Message });
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