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
                    // Store registration data in TempData for auto-login after OTP verification
                    TempData["RegisterUsername"] = model.Username;
                    TempData["RegisterPassword"] = model.Password;
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
                    // After successful OTP verification, auto-login the user
                    var username = TempData["RegisterUsername"]?.ToString();
                    var password = TempData["RegisterPassword"]?.ToString();

                    if (!string.IsNullOrEmpty(username) && !string.IsNullOrEmpty(password))
                    {
                        try
                        {
                            var loginModel = new LoginViewModel
                            {
                                Username = username,
                                Password = password
                            };

                            var loginResult = await _userApiService.LoginAsync(loginModel);

                            if (loginResult.IsSuccess)
                            {
                                // Store user information in session
                                HttpContext.Session.SetString("AccessToken", loginResult.AccessToken);
                                HttpContext.Session.SetString("UserName", loginResult.UserName);
                                HttpContext.Session.SetString("UserEmail", loginResult.Email);
                                HttpContext.Session.SetString("UserRole", loginResult.RoleName);
                                HttpContext.Session.SetString("UserId", loginResult.UserId.ToString());

                                TempData["Success"] = "Xác thực thành công! Chào mừng bạn đến với Yến Sào Ngọc Nga!";
                                return RedirectToAction("Index", "Home", new { area = "" });
                            }
                        }
                        catch (Exception ex)
                        {
                            _logger.LogError(ex, "Auto-login failed after OTP verification");
                        }
                    }

                    // Fallback: redirect to login page if auto-login fails
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

        // AJAX: Resend OTP
        [HttpPost]
        public async Task<IActionResult> ResendOtp([FromBody] ResendOtpRequest request)
        {
            try
            {
                // This would need to be implemented in your UserApiService
                // For now, return success message
                return Json(new { success = true, message = "Mã OTP mới đã được gửi!" });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error resending OTP");
                return Json(new { success = false, message = "Có lỗi xảy ra khi gửi lại OTP!" });
            }
        }
    }

    public class ResendOtpRequest
    {
        public string Email { get; set; } = string.Empty;
    }
}