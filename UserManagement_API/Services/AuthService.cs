using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using Microsoft.Extensions.Configuration;
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
        private readonly IConfiguration _configuration;

        public AuthService(IUserRepository userRepository, IEmailService emailService, IConfiguration configuration)
        {
            _userRepository = userRepository;
            _emailService = emailService;
            _configuration = configuration;
        }

        public async Task<RegisterResponseDto> RegisterAsync(RegisterDto registerDto)
        {
            if (await _userRepository.ExistsByUsernameAsync(registerDto.Username))
            {
                return new RegisterResponseDto
                {
                    Success = false,
                    Message = "Username already exists"
                };
            }

            if (await _userRepository.ExistsByEmailAsync(registerDto.Email))
            {
                return new RegisterResponseDto
                {
                    Success = false,
                    Message = "Email already exists"
                };
            }

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

            // Verify password based on role
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

            // 🆕 Generate JWT Token
            var authClaims = new List<Claim>
        {
            new Claim(ClaimTypes.Name, user.UserId.ToString()),
            new Claim(ClaimTypes.Email, user.Email),
            new Claim(ClaimTypes.Role, user.Role.RoleName),
            new Claim(JwtRegisteredClaimNames.Jti, Guid.NewGuid().ToString()),
            new Claim("username", user.Username),
            new Claim("fullName", user.FullName)
        };

            var token = JwtHelper.CreateToken(authClaims, _configuration);
            var refreshToken = JwtHelper.GenerateRefreshToken();

            return new AuthResponseDto
            {
                UserId = user.UserId,
                Username = user.Username,
                FullName = user.FullName,
                Email = user.Email,
                RoleName = user.Role.RoleName,
                IsActive = user.IsActive,
                Message = "Login successful",
                AccessToken = new JwtSecurityTokenHandler().WriteToken(token), // 🆕 JWT Token
                RefreshToken = refreshToken, // 🆕 Refresh Token
                TokenExpiry = token.ValidTo // 🆕 Token expiry
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

        public async Task<ForgotPasswordResponseDto> ForgotPasswordAsync(ForgotPasswordDto forgotPasswordDto)
        {
            var user = await _userRepository.GetByEmailAsync(forgotPasswordDto.Email);
            if (user == null)
            {
                return new ForgotPasswordResponseDto
                {
                    Success = false,
                    Message = "Email không tồn tại trong hệ thống"
                };
            }

            if (!user.IsActive)
            {
                return new ForgotPasswordResponseDto
                {
                    Success = false,
                    Message = "Tài khoản chưa được kích hoạt"
                };
            }

            // Generate reset OTP (6 số ngẫu nhiên)
            var resetOtp = new Random().Next(100000, 999999).ToString();
            await _userRepository.SaveResetOtpAsync(forgotPasswordDto.Email, resetOtp);

            //Gửi email với mã reset OTP
            string emailBody = $@"
        <html>
        <body style='font-family: Arial, sans-serif; padding: 20px;'>
            <h2>🔐 Đặt Lại Mật Khẩu</h2>
            <p>Chào <strong>{user.FullName}</strong>,</p>
            <p>Bạn đã yêu cầu đặt lại mật khẩu cho tài khoản <strong>{user.Username}</strong>.</p>
            <div style='text-align: center; margin: 20px 0;'>
                <p>Mã OTP đặt lại mật khẩu của bạn là:</p>
                <div style='background-color: #f44336; color: white; padding: 15px; border-radius: 8px; font-size: 24px; font-weight: bold; letter-spacing: 3px;'>
                    {resetOtp}
                </div>
            </div>
            <p><strong>⏰ Mã này sẽ hết hạn sau 15 phút.</strong></p>
            <p>Nếu bạn không yêu cầu đặt lại mật khẩu, vui lòng bỏ qua email này.</p>
            <br>
            <p>Trân trọng,<br>User Management Team</p>
        </body>
        </html>";

            try
            {
                await _emailService.SendVerifyEmailAsync(forgotPasswordDto.Email, "Đặt lại mật khẩu", emailBody);

                return new ForgotPasswordResponseDto
                {
                    Success = true,
                    Message = "Mã OTP đặt lại mật khẩu đã được gửi đến email của bạn"
                };
            }
            catch (Exception ex)
            {
                return new ForgotPasswordResponseDto
                {
                    Success = false,
                    Message = $"Lỗi gửi email: {ex.Message}"
                };
            }
        }

        public async Task<ForgotPasswordResponseDto> ResetPasswordAsync(ResetPasswordDto resetPasswordDto)
        {
            var user = await _userRepository.GetByEmailAsync(resetPasswordDto.Email);
            if (user == null)
            {
                return new ForgotPasswordResponseDto
                {
                    Success = false,
                    Message = "Email không tồn tại trong hệ thống"
                };
            }

            var isValidOtp = await _userRepository.VerifyResetOtpAsync(resetPasswordDto.Email, resetPasswordDto.Otp);
            if (!isValidOtp)
            {
                return new ForgotPasswordResponseDto
                {
                    Success = false,
                    Message = "Mã OTP không hợp lệ hoặc đã hết hạn"
                };
            }

            var hashedPassword = user.RoleId == 3 ?
                PasswordHelperStatic.HashPassword(resetPasswordDto.NewPassword) :
                resetPasswordDto.NewPassword;

            user.Password = hashedPassword;
            user.Otp = null; // Clear reset OTP
            user.OtpCreatedAt = null;
            user.UpdatedAt = DateTime.UtcNow;

            await _userRepository.UpdateAsync(user);

            return new ForgotPasswordResponseDto
            {
                Success = true,
                Message = "Mật khẩu đã được đặt lại thành công"
            };
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