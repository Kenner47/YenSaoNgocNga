using UserManagement_API.Models.DTOs;
using UserManagement_API.Models.Entities;

namespace UserManagement_API.Repositories.IRepository
{
    public interface IUserRepository
    {
        Task<User?> GetByUsernameAsync(string username);
        Task<User?> GetByEmailAsync(string email);
        Task<User> CreateAsync(User user);
        Task<User> UpdateAsync(User user);
        Task<bool> ExistsByUsernameAsync(string username);
        Task<bool> ExistsByEmailAsync(string email);
        Task<User?> GetByIdWithRoleAsync(int id);
        Task<User?> GetByIdAsync(int id);
        Task<IEnumerable<User>> GetAllUsersWithRoleAsync();
        Task<bool> DeleteAsync(int id);
        Task<IEnumerable<User>> SearchUsersAsync(string? keyword);
        Task<UserStatisticsDto> GetUserStatisticsAsync();

        // OTP methods
        Task SaveOtpAsync(string email, string otp);
        Task<bool> VerifyOtpAsync(string email, string otp);
        Task<(string? OtpCode, DateTime? OtpCreatedAt)> GetOtpInfoAsync(string email);

        // RESET PASSWORD METHODS (reuse OTP fields)
        Task SaveResetOtpAsync(string email, string otp);
        Task<bool> VerifyResetOtpAsync(string email, string otp);

    }
}