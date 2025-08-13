using Microsoft.AspNetCore.Mvc;
using YenSaoNgocNga_MVC.Areas.UserManagement.Models;
using YenSaoNgocNga_MVC.Areas.UserManagement.Services;

namespace YenSaoNgocNga_MVC.Areas.UserManagement.Controllers
{
    [Area("UserManagement")]
    public class UserController : Controller
    {
        private readonly IUserApiService _userApiService;
        private readonly ILogger<UserController> _logger;

        public UserController(IUserApiService userApiService, ILogger<UserController> logger)
        {
            _userApiService = userApiService;
            _logger = logger;
        }

        // GET: /UserManagement/User/Profile
        public async Task<IActionResult> Profile()
        {
            var userId = HttpContext.Session.GetString("UserId");
            if (string.IsNullOrEmpty(userId))
            {
                TempData["Error"] = "Vui lòng đăng nhập để xem thông tin cá nhân!";
                return RedirectToAction("Login", "Auth");
            }

            try
            {
                var profileViewModel = await _userApiService.GetUserByIdAsync(int.Parse(userId));

                if (profileViewModel == null)
                {
                    TempData["Error"] = "Không tìm thấy thông tin người dùng!";
                    return RedirectToAction("Index", "Home", new { area = "" });
                }

                return View(profileViewModel);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error loading user profile");
                TempData["Error"] = "Có lỗi xảy ra khi tải thông tin cá nhân!";
                return RedirectToAction("Index", "Home", new { area = "" });
            }
        }

        // POST: /UserManagement/User/UpdateProfile
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> UpdateProfile(UserProfileViewModel model)
        {
            var userId = HttpContext.Session.GetString("UserId");
            if (string.IsNullOrEmpty(userId))
            {
                TempData["Error"] = "Vui lòng đăng nhập để cập nhật thông tin!";
                return RedirectToAction("Login", "Auth");
            }

            // Debug logging
            _logger.LogInformation("UpdateProfile called with model: {@Model}", model);
            _logger.LogInformation("ModelState.IsValid: {IsValid}", ModelState.IsValid);

            if (!ModelState.IsValid)
            {
                foreach (var error in ModelState)
                {
                    _logger.LogWarning("ModelState error - Key: {Key}, Errors: {Errors}",
                        error.Key, string.Join(", ", error.Value.Errors.Select(e => e.ErrorMessage)));
                }

                TempData["Error"] = "Dữ liệu không hợp lệ. Vui lòng kiểm tra lại!";
                var reloadedModel = await _userApiService.GetUserByIdAsync(int.Parse(userId));
                return View("Profile", reloadedModel ?? model);
            }

            // Ensure the model ID is set correctly
            if (model.Id == 0)
            {
                model.Id = int.Parse(userId);
            }

            try
            {
                var success = await _userApiService.UpdateUserAsync(model.Id, model);

                if (success)
                {
                    // Cập nhật session với tên mới
                    HttpContext.Session.SetString("UserName", model.FullName ?? model.Username);

                    TempData["Success"] = "Cập nhật thông tin cá nhân thành công!";
                    return RedirectToAction("Profile");
                }
                else
                {
                    TempData["Error"] = "Không thể cập nhật thông tin. Vui lòng thử lại!";
                    var reloadedModel = await _userApiService.GetUserByIdAsync(int.Parse(userId));
                    return View("Profile", reloadedModel ?? model);
                }
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error updating user profile");
                TempData["Error"] = "Có lỗi xảy ra khi cập nhật thông tin!";

                var reloadedModel = await _userApiService.GetUserByIdAsync(int.Parse(userId));
                return View("Profile", reloadedModel ?? model);
            }
        }

        // POST: /UserManagement/User/ChangePassword
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> ChangePassword(ChangePasswordViewModel model)
        {
            var userId = HttpContext.Session.GetString("UserId");
            if (string.IsNullOrEmpty(userId))
            {
                return Json(new { success = false, message = "Vui lòng đăng nhập để thực hiện chức năng này!" });
            }

            if (!ModelState.IsValid)
            {
                var errors = ModelState.Values.SelectMany(v => v.Errors).Select(e => e.ErrorMessage).ToList();
                return Json(new { success = false, message = string.Join("\n", errors) });
            }

            try
            {
                var success = await _userApiService.ChangePasswordAsync(model);

                if (success)
                {
                    return Json(new { success = true, message = "Đổi mật khẩu thành công!" });
                }
                else
                {
                    return Json(new { success = false, message = "Mật khẩu hiện tại không đúng. Vui lòng thử lại!" });
                }
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error changing password");
                return Json(new { success = false, message = "Có lỗi xảy ra khi đổi mật khẩu!" });
            }
        }
    }
}