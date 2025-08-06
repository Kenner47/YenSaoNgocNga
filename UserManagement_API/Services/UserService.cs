using UserManagement_API.Helpers;
using UserManagement_API.Models.DTOs;
using UserManagement_API.Models.Entities;
using UserManagement_API.Repositories.IRepository;
using UserManagement_API.Services.IService;

namespace UserManagement_API.Services
{
    public class UserService : IUserService
    {
        private readonly IUserRepository _userRepository;

        public UserService(IUserRepository userRepository)
        {
            _userRepository = userRepository;
        }

        // Admin functions
        public async Task<IEnumerable<UserDto>> GetAllUsersAsync()
        {
            var users = await _userRepository.GetAllUsersWithRoleAsync();
            return users.Select(MapToUserDto);
        }

        public async Task<UserDto?> GetUserByIdAsync(int id)
        {
            var user = await _userRepository.GetByIdWithRoleAsync(id);
            return user != null ? MapToUserDto(user) : null;
        }

        public async Task<UserDto?> UpdateUserAsync(int id, AdminUpdateUserDto updateDto)
        {
            var user = await _userRepository.GetByIdAsync(id);
            if (user == null) return null;

            // Update user properties
            user.FullName = updateDto.FullName;
            user.Email = updateDto.Email;
            user.PhoneNumber = updateDto.PhoneNumber ?? "";
            user.Address = updateDto.Address ?? "";
            user.DateOfBirth = updateDto.DateOfBirth ?? user.DateOfBirth;
            user.Sex = updateDto.Sex ?? user.Sex;
            user.RoleId = updateDto.RoleId;
            user.IsActive = updateDto.IsActive;
            user.UpdatedAt = DateTime.UtcNow;

            var updatedUser = await _userRepository.UpdateAsync(user);
            var userWithRole = await _userRepository.GetByIdWithRoleAsync(updatedUser.UserId);

            return userWithRole != null ? MapToUserDto(userWithRole) : null;
        }

        public async Task<bool> ToggleUserStatusAsync(int id)
        {
            var user = await _userRepository.GetByIdAsync(id);
            if (user == null) return false;

            user.IsActive = !user.IsActive;
            user.UpdatedAt = DateTime.UtcNow;

            await _userRepository.UpdateAsync(user);
            return true;
        }

        public async Task<bool> DeleteUserAsync(int id)
        {
            var user = await _userRepository.GetByIdAsync(id);
            if (user == null) return false;

            // Soft delete - just deactivate
            user.IsActive = false;
            user.UpdatedAt = DateTime.UtcNow;

            await _userRepository.UpdateAsync(user);
            return true;
        }

        // User functions
        public async Task<UserDto?> GetMyProfileAsync(int userId)
        {
            var user = await _userRepository.GetByIdWithRoleAsync(userId);
            return user != null ? MapToUserDto(user) : null;
        }

        public async Task<UserDto?> UpdateMyProfileAsync(int userId, UpdateUserDto updateDto)
        {
            var user = await _userRepository.GetByIdAsync(userId);
            if (user == null) return null;

            // Update only allowed fields for regular users
            user.FullName = updateDto.FullName;
            user.Email = updateDto.Email;
            user.PhoneNumber = updateDto.PhoneNumber ?? user.PhoneNumber;
            user.Address = updateDto.Address ?? user.Address;
            user.DateOfBirth = updateDto.DateOfBirth ?? user.DateOfBirth;
            user.Sex = updateDto.Sex ?? user.Sex;
            user.UpdatedAt = DateTime.UtcNow;

            var updatedUser = await _userRepository.UpdateAsync(user);
            var userWithRole = await _userRepository.GetByIdWithRoleAsync(updatedUser.UserId);

            return userWithRole != null ? MapToUserDto(userWithRole) : null;
        }

        public async Task<bool> ChangePasswordAsync(int userId, ChangePasswordDto changePasswordDto)
        {
            var user = await _userRepository.GetByIdAsync(userId);
            if (user == null) return false;

            // Verify current password
            bool isCurrentPasswordValid = user.RoleId == 3 ?
                PasswordHelperStatic.VerifyPassword(changePasswordDto.CurrentPassword, user.Password) :
                (changePasswordDto.CurrentPassword == user.Password);

            if (!isCurrentPasswordValid) return false;

            // Update password
            var newHashedPassword = user.RoleId == 3 ?
                PasswordHelperStatic.HashPassword(changePasswordDto.NewPassword) :
                changePasswordDto.NewPassword;

            user.Password = newHashedPassword;
            user.UpdatedAt = DateTime.UtcNow;

            await _userRepository.UpdateAsync(user);
            return true;
        }

        private static UserDto MapToUserDto(User user)
        {
            return new UserDto
            {
                UserId = user.UserId,
                Username = user.Username,
                FullName = user.FullName,
                Email = user.Email,
                PhoneNumber = user.PhoneNumber,
                Address = user.Address,
                DateOfBirth = user.DateOfBirth,
                Sex = user.Sex,
                RoleName = user.Role.RoleName,
                IsActive = user.IsActive,
                CreatedAt = user.CreatedAt
            };
        }
    }
}