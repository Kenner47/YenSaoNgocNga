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
        private readonly IEmailService _emailService;

        public AuthService(IUserRepository userRepository, IEmailService emailService)
        {
            _userRepository = userRepository;
            _emailService = emailService;
        }

        public async Task<RegisterResponseDto> RegisterAsync(RegisterDto registerDto)
        {
            // Check username exists
            if (await _userRepository.ExistsByUsernameAsync(registerDto.Username))
            {
                return new RegisterResponseDto
                {
                    Success = false,
                    Message = "Username already exists"
                };
            }

            // Check email exists
            if (await _userRepository.ExistsByEmailAsync(registerDto.Email))
            {
                return new RegisterResponseDto
                {
                    Success = false,
                    Message = "Email already exists"
                };
            }

            // Validate email format
            if (!IsValidEmail(registerDto.Email))
            {
                return new RegisterResponseDto
                {
                    Success = false,
                    Message = "Email không hợp lệ"
                };
            }

            // Hash password
            var hashedPassword = PasswordHelperStatic.HashPassword(registerDto.Password);

            // Create new user
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
                RoleId = 3, // User role
                IsActive = false,
                Otp = null,
                OtpCreatedAt = null,
                CreatedAt = DateTime.UtcNow,
                UpdatedAt = DateTime.UtcNow
            };

            var createdUser = await _userRepository.CreateAsync(user);

            // Generate và gửi OTP
            string otp = new Random().Next(100000, 999999).ToString();
            await _userRepository.SaveOtpAsync(registerDto.Email, otp);

            string emailBody = $"Mã OTP của bạn là: <b>{otp}</b>. Vui lòng nhập mã này để kích hoạt tài khoản.";
            try
            {
                await _emailService.SendVerifyEmailAsync(registerDto.Email, "Xác nhận đăng ký", emailBody);
            }
            catch (Exception ex)
            {
                return new RegisterResponseDto
                {
                    Success = false,
                    Message = $"Lỗi gửi email: {ex.Message}"
                };
            }

            return new RegisterResponseDto
            {
                UserId = createdUser.UserId,
                Username = createdUser.Username,
                Email = createdUser.Email,
                Success = true,
                Message = "Đăng kí thành công. Vui lòng kiểm tra email để nhập mã OTP."
            };
        }

        public async Task<bool> ActivateAccountAsync(VerifyOtpDto verifyOtpDto)
        {
            var user = await _userRepository.GetByEmailAsync(verifyOtpDto.Email);
            if (user == null) return false;

            // Verify OTP
            var otpInfo = await _userRepository.GetOtpInfoAsync(verifyOtpDto.Email);
            if (otpInfo.OtpCode == null || otpInfo.OtpCreatedAt == null)
            {
                return false;
            }

            var elapsedTime = DateTime.UtcNow - otpInfo.OtpCreatedAt.Value;
            if (elapsedTime.TotalMinutes > 5)
            {
                return false;
            }

            if (otpInfo.OtpCode != verifyOtpDto.Otp)
            {
                return false;
            }

            user.IsActive = true;
            user.Otp = null; // Clear OTP after successful
            user.OtpCreatedAt = null;
            user.UpdatedAt = DateTime.UtcNow;
            await _userRepository.UpdateAsync(user);

            return true;
        }

        public async Task<RegisterResponseDto> ResendOtpAsync(string email)
        {
            var user = await _userRepository.GetByEmailAsync(email);
            if (user == null)
            {
                return new RegisterResponseDto
                {
                    Success = false,
                    Message = "Email không tồn tại"
                };
            }

            // resend OTP
            if (user.IsActive)
            {
                return new RegisterResponseDto
                {
                    Success = false,
                    Message = "Tài khoản đã được kích hoạt"
                };
            }

            var otpInfo = await _userRepository.GetOtpInfoAsync(email);

            if (otpInfo.OtpCode == null || otpInfo.OtpCreatedAt == null ||
                (DateTime.UtcNow - otpInfo.OtpCreatedAt.Value).TotalMinutes > 5)
            {
                string otp = new Random().Next(100000, 999999).ToString();
                await _userRepository.SaveOtpAsync(email, otp);

                string emailBody = $"Mã OTP của bạn là: <b>{otp}</b>. Vui lòng nhập mã này để kích hoạt tài khoản.";
                try
                {
                    await _emailService.SendVerifyEmailAsync(email, "Xác nhận đăng ký", emailBody);
                }
                catch (Exception ex)
                {
                    return new RegisterResponseDto
                    {
                        Success = false,
                        Message = $"Lỗi gửi email: {ex.Message}"
                    };
                }

                return new RegisterResponseDto
                {
                    Success = true,
                    Message = "OTP mới đã được gửi đến email của bạn."
                };
            }

            return new RegisterResponseDto
            {
                Success = false,
                Message = "OTP chưa hết hạn"
            };
        }

        public async Task<AuthResponseDto> LoginAsync(LoginDto loginDto)
        {
            var user = await _userRepository.GetByUsernameAsync(loginDto.Username);
            if (user == null)
            {
                return new AuthResponseDto
                {
                    Message = "Invalid username or password"
                };
            }

            if (!user.IsActive)
            {
                return new AuthResponseDto
                {
                    Message = "Tài khoản chưa được kích hoạt. Vui lòng kiểm tra email để nhập mã OTP."
                };
            }

            // Verify password
            bool passwordValid = user.RoleId == 3 ?
                PasswordHelperStatic.VerifyPassword(loginDto.Password, user.Password) :
                (loginDto.Password == user.Password);

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

        public async Task<bool> LogoutAsync(int userId)
        {
            try
            {
                // Trong trường hợp đơn giản, chỉ cần log thông tin logout
                // Trong thực tế có thể cần:
                // - Invalidate JWT token (nếu dùng JWT)
                // - Clear session (nếu dùng session)
                // - Log audit trail
                // - Update last logout time

                var user = await _userRepository.GetByIdAsync(userId);
                if (user == null) return false;

                // Có thể log logout time
                user.UpdatedAt = DateTime.UtcNow; // Update last activity
                await _userRepository.UpdateAsync(user);

                // Log cho debugging
                Console.WriteLine($"User {user.Username} (ID: {userId}) logged out at {DateTime.UtcNow}");

                return true;
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Logout failed for userId {userId}: {ex.Message}");
                return false;
            }
        }

        private bool IsValidEmail(string email)
        {
            try
            {
                var addr = new System.Net.Mail.MailAddress(email);
                return addr.Address == email;
            }
            catch
            {
                return false;
            }
        }
    }
}