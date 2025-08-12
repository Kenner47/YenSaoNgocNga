using YenSaoNgocNga_MVC.Areas.UserManagement.Models;

namespace YenSaoNgocNga_MVC.Areas.UserManagement.Services
{
    public interface IUserApiService
    {
        Task<AuthResult> LoginAsync(LoginViewModel model);
        Task<AuthResult> RegisterAsync(RegisterViewModel model);
        Task<AuthResult> VerifyOtpAsync(VerifyOtpViewModel model);
    }

    public class AuthResult
    {
        public bool IsSuccess { get; set; }
        public string Message { get; set; } = string.Empty;
        public string AccessToken { get; set; } = string.Empty;
        public string UserName { get; set; } = string.Empty;
        public string Email { get; set; } = string.Empty;
        public string RoleName { get; set; } = string.Empty; // 🔥 Thêm RoleName
        public int UserId { get; set; } // 🔥 Thêm UserId
    }
}