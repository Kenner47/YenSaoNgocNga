using UserManagement_API.Helpers;
using UserManagement_API.Models.DTOs;
using UserManagement_API.Models.Entities;
using UserManagement_API.Repositories.IRepository;
using UserManagement_API.Services.IService;

namespace UserManagement_API.Services
{
    public class AuthService : IAuthService
    {
        private readonly IUserRepository _userRepository;

        public AuthService(IUserRepository userRepository)
        {
            _userRepository = userRepository;
        }

        public async Task<AuthResponseDto> RegisterAsync(RegisterDto registerDto)
        {
            // Check if username already exists
            if (await _userRepository.ExistsByUsernameAsync(registerDto.Username))
            {
                return new AuthResponseDto
                {
                    Message = "Username already exists"
                };
            }

            // Check if email already exists
            if (await _userRepository.ExistsByEmailAsync(registerDto.Email))
            {
                return new AuthResponseDto
                {
                    Message = "Email already exists"
                };
            }

            // Hash password using Identity PasswordHasher with salt (chỉ cho User role)
            var hashedPassword = PasswordHelperStatic.HashPassword(registerDto.Password);

            // Create new user with default User role (RoleId = 3)
            var user = new User
            {
                Username = registerDto.Username,
                Password = hashedPassword,
                FullName = registerDto.FullName,
                Email = registerDto.Email,
                PhoneNumber = registerDto.PhoneNumber ?? "",
                Address = registerDto.Address ?? "",
                DateOfBirth = registerDto.DateOfBirth ?? DateOnly.FromDateTime(DateTime.Now),
                Sex = registerDto.Sex ?? "",
                RoleId = 3, // Mặc định luôn là User role
                IsActive = true,
                CreatedAt = DateTime.UtcNow,
                UpdatedAt = DateTime.UtcNow
            };

            var createdUser = await _userRepository.CreateAsync(user);
            var userWithRole = await _userRepository.GetByIdWithRoleAsync(createdUser.UserId);

            return new AuthResponseDto
            {
                UserId = userWithRole.UserId,
                Username = userWithRole.Username,
                FullName = userWithRole.FullName,
                Email = userWithRole.Email,
                RoleName = userWithRole.Role.RoleName,
                IsActive = userWithRole.IsActive,
                Message = "User registered successfully"
            };
        }

        public async Task<AuthResponseDto> LoginAsync(LoginDto loginDto)
        {
            // Find user by username
            var user = await _userRepository.GetByUsernameAsync(loginDto.Username);
            if (user == null)
            {
                return new AuthResponseDto
                {
                    Message = "Invalid username or password"
                };
            }

            // Check if user is active
            if (!user.IsActive)
            {
                return new AuthResponseDto
                {
                    Message = "Account is deactivated"
                };
            }

            // Verify password based on role
            bool passwordValid = false;

            if (user.RoleId == 3) // User role - sử dụng hashed password
            {
                passwordValid = PasswordHelperStatic.VerifyPassword(loginDto.Password, user.Password);
            }
            else // Admin (RoleId = 1) và Employee (RoleId = 2) - sử dụng plain text
            {
                passwordValid = (loginDto.Password == user.Password);
            }

            if (!passwordValid)
            {
                return new AuthResponseDto
                {
                    Message = "Invalid username or password"
                };
            }

            return new AuthResponseDto
            {
                UserId = user.UserId,
                Username = user.Username,
                FullName = user.FullName,
                Email = user.Email,
                RoleName = user.Role.RoleName,
                IsActive = user.IsActive,
                Message = "Login successful"
            };
        }
    }
}