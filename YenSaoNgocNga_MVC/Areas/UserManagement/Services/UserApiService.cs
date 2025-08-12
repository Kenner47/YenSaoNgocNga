using YenSaoNgocNga_MVC.Areas.UserManagement.Models;
using YenSaoNgocNga_MVC.Models.Configuration;
using System.Text;
using System.Text.Json;

namespace YenSaoNgocNga_MVC.Areas.UserManagement.Services
{
    public class UserApiService : IUserApiService
    {
        private readonly HttpClient _httpClient;
        private readonly JsonSerializerOptions _jsonOptions;

        public UserApiService(HttpClient httpClient)
        {
            _httpClient = httpClient;
            _httpClient.BaseAddress = new Uri(ApiSettings.ApiUrls.UserManagementApi);
            _jsonOptions = new JsonSerializerOptions
            {
                PropertyNameCaseInsensitive = true
            };
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
                        UserName = root.GetProperty("username").GetString() ?? "",
                        Email = root.GetProperty("email").GetString() ?? "",
                        RoleName = root.GetProperty("roleName").GetString() ?? "", // 🔥 Lấy role
                        UserId = root.GetProperty("userId").GetInt32() // 🔥 Lấy userId
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
                return new AuthResult
                {
                    IsSuccess = false,
                    Message = ex.Message
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
                return new AuthResult
                {
                    IsSuccess = false,
                    Message = ex.Message
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

                var response = await _httpClient.PostAsync("/api/auth/verify-otp", content);

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
                return new AuthResult
                {
                    IsSuccess = false,
                    Message = ex.Message
                };
            }
        }
    }
}