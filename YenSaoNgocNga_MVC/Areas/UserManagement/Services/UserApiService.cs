using YenSaoNgocNga_MVC.Areas.UserManagement.Models;
using YenSaoNgocNga_MVC.Models.Configuration;
using System.Text;
using System.Text.Json;
using System.Net.Http.Headers;

namespace YenSaoNgocNga_MVC.Areas.UserManagement.Services
{
    public class UserApiService : IUserApiService
    {
        private readonly HttpClient _httpClient;
        private readonly JsonSerializerOptions _jsonOptions;
        private readonly IHttpContextAccessor _httpContextAccessor;
        private readonly ILogger<UserApiService> _logger;

        public UserApiService(HttpClient httpClient, IHttpContextAccessor httpContextAccessor, ILogger<UserApiService> logger)
        {
            _httpClient = httpClient;
            _httpContextAccessor = httpContextAccessor;
            _logger = logger;
            _httpClient.BaseAddress = new Uri(ApiSettings.ApiUrls.UserManagementApi);
            _jsonOptions = new JsonSerializerOptions
            {
                PropertyNameCaseInsensitive = true
            };
        }

        private void SetAuthorizationHeader()
        {
            var token = _httpContextAccessor.HttpContext?.Session.GetString("AccessToken");
            if (!string.IsNullOrEmpty(token))
            {
                _httpClient.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", token);
            }
        }

        public async Task<AuthResult> LoginAsync(LoginViewModel model)
        {
            try
            {
                var loginData = new
                {
                    username = model.Username,
                    password = model.Password
                };

                var json = JsonSerializer.Serialize(loginData);
                var content = new StringContent(json, Encoding.UTF8, "application/json");

                var response = await _httpClient.PostAsync("/api/auth/login", content);
                var responseContent = await response.Content.ReadAsStringAsync();

                if (response.IsSuccessStatusCode)
                {
                    using var document = JsonDocument.Parse(responseContent);
                    var root = document.RootElement;

                    return new AuthResult
                    {
                        IsSuccess = true,
                        Message = "Đăng nhập thành công",
                        AccessToken = root.GetProperty("accessToken").GetString() ?? "",
                        UserName = root.GetProperty("fullName").GetString() ?? root.GetProperty("username").GetString() ?? "",
                        Email = root.GetProperty("email").GetString() ?? "",
                        RoleName = root.GetProperty("roleName").GetString() ?? "",
                        UserId = root.GetProperty("userId").GetInt32()
                    };
                }

                return new AuthResult
                {
                    IsSuccess = false,
                    Message = "Tên đăng nhập hoặc mật khẩu không đúng"
                };
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error during login");
                return new AuthResult
                {
                    IsSuccess = false,
                    Message = "Có lỗi xảy ra khi đăng nhập"
                };
            }
        }

        public async Task<AuthResult> RegisterAsync(RegisterViewModel model)
        {
            try
            {
                var registerData = new
                {
                    username = model.Username,
                    email = model.Email,
                    password = model.Password,
                    fullName = model.FullName,
                    phoneNumber = model.PhoneNumber,
                    address = model.Address
                };

                var json = JsonSerializer.Serialize(registerData);
                var content = new StringContent(json, Encoding.UTF8, "application/json");

                var response = await _httpClient.PostAsync("/api/auth/register", content);

                if (response.IsSuccessStatusCode)
                {
                    return new AuthResult
                    {
                        IsSuccess = true,
                        Message = "Đăng ký thành công"
                    };
                }

                return new AuthResult
                {
                    IsSuccess = false,
                    Message = "Đăng ký thất bại"
                };
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error during registration");
                return new AuthResult
                {
                    IsSuccess = false,
                    Message = "Có lỗi xảy ra khi đăng ký"
                };
            }
        }

        public async Task<AuthResult> VerifyOtpAsync(VerifyOtpViewModel model)
        {
            try
            {
                var otpData = new
                {
                    email = model.Email,
                    otp = model.Otp
                };

                var json = JsonSerializer.Serialize(otpData);
                var content = new StringContent(json, Encoding.UTF8, "application/json");

                var response = await _httpClient.PostAsync("/api/auth/activate-account", content);

                if (response.IsSuccessStatusCode)
                {
                    return new AuthResult
                    {
                        IsSuccess = true,
                        Message = "Xác thực thành công"
                    };
                }

                return new AuthResult
                {
                    IsSuccess = false,
                    Message = "Mã OTP không hợp lệ"
                };
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error during OTP verification");
                return new AuthResult
                {
                    IsSuccess = false,
                    Message = "Có lỗi xảy ra khi xác thực OTP"
                };
            }
        }

        public async Task<UserProfileViewModel?> GetUserByIdAsync(int userId)
        {
            try
            {
                SetAuthorizationHeader();

                var response = await _httpClient.GetAsync("api/User/profile");

                if (response.IsSuccessStatusCode)
                {
                    var json = await response.Content.ReadAsStringAsync();

                    using var document = JsonDocument.Parse(json);
                    var root = document.RootElement;

                    var profile = new UserProfileViewModel
                    {
                        Id = root.GetProperty("userId").GetInt32(),
                        Username = root.GetProperty("username").GetString() ?? "",
                        Email = root.GetProperty("email").GetString() ?? "",
                        FullName = root.GetProperty("fullName").GetString() ?? "",
                        PhoneNumber = root.TryGetProperty("phoneNumber", out var phoneProp) ? phoneProp.GetString() : null,
                        Address = root.TryGetProperty("address", out var addrProp) ? addrProp.GetString() : null,
                        DateOfBirth = root.TryGetProperty("dateOfBirth", out var dobProp) &&
                                     DateOnly.TryParse(dobProp.GetString(), out var dob) ? dob : null,
                        Sex = root.TryGetProperty("sex", out var sexProp) ? sexProp.GetString() : null,
                        RoleName = root.GetProperty("roleName").GetString() ?? "",
                        IsActive = root.GetProperty("isActive").GetBoolean(),
                        CreatedAt = root.GetProperty("createdAt").GetDateTime()
                    };

                    return profile;
                }
                else
                {
                    _logger.LogWarning("Failed to get user profile. Status: {StatusCode}", response.StatusCode);
                }

                return null;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error getting user profile");
                return null;
            }
        }

        public async Task<bool> UpdateUserAsync(int userId, UserProfileViewModel userProfile)
        {
            try
            {
                if (string.IsNullOrWhiteSpace(userProfile.FullName) || string.IsNullOrWhiteSpace(userProfile.Email))
                {
                    _logger.LogWarning("FullName or Email is empty");
                    return false;
                }

                SetAuthorizationHeader();

                var updateData = new
                {
                    FullName = userProfile.FullName.Trim(),
                    Email = userProfile.Email.Trim(),
                    PhoneNumber = userProfile.PhoneNumber?.Trim(),
                    Address = userProfile.Address?.Trim(),
                    DateOfBirth = userProfile.DateOfBirth,
                    Sex = userProfile.Sex?.Trim()
                };

                var json = JsonSerializer.Serialize(updateData, _jsonOptions);
                var content = new StringContent(json, Encoding.UTF8, "application/json");

                var response = await _httpClient.PutAsync("api/User/profile", content);

                if (response.IsSuccessStatusCode)
                {
                    return true;
                }
                else
                {
                    var errorContent = await response.Content.ReadAsStringAsync();
                    _logger.LogWarning("Failed to update user profile. Status: {StatusCode}, Content: {Content}",
                        response.StatusCode, errorContent);
                    return false;
                }
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error updating user profile");
                return false;
            }
        }

        public async Task<bool> ChangePasswordAsync(ChangePasswordViewModel changePasswordModel)
        {
            try
            {
                SetAuthorizationHeader();

                var changePasswordData = new
                {
                    CurrentPassword = changePasswordModel.CurrentPassword,
                    NewPassword = changePasswordModel.NewPassword,
                    ConfirmPassword = changePasswordModel.ConfirmPassword
                };

                var json = JsonSerializer.Serialize(changePasswordData, _jsonOptions);
                var content = new StringContent(json, Encoding.UTF8, "application/json");

                var response = await _httpClient.PutAsync("api/User/profile/change-password", content);

                if (response.IsSuccessStatusCode)
                {
                    return true;
                }
                else
                {
                    var errorContent = await response.Content.ReadAsStringAsync();
                    _logger.LogWarning("Failed to change password. Status: {StatusCode}, Content: {Content}",
                        response.StatusCode, errorContent);
                    return false;
                }
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error changing password");
                return false;
            }
        }
    }
}