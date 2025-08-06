using UserManagement_API.Models.DTOs;

namespace UserManagement_API.Services.IService
{
    public interface IUserService
    {
        // Admin
        Task<IEnumerable<UserDto>> GetAllUsersAsync();
        Task<UserDto?> GetUserByIdAsync(int id);
        Task<UserDto?> UpdateUserAsync(int id, AdminUpdateUserDto updateDto);
        Task<bool> ToggleUserStatusAsync(int id);
        Task<bool> DeleteUserAsync(int id);
        Task<IEnumerable<UserDto>> SearchUsersAsync(string? keyword);
        Task<UserStatisticsDto> GetUserStatisticsAsync();

        // User
        Task<UserDto?> GetMyProfileAsync(int userId);
        Task<UserDto?> UpdateMyProfileAsync(int userId, UpdateUserDto updateDto);
        Task<bool> ChangePasswordAsync(int userId, ChangePasswordDto changePasswordDto);

    }
}
