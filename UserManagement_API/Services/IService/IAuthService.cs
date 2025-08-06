using UserManagement_API.Models.DTOs;

namespace UserManagement_API.Services.IService
{
    public interface IAuthService
    {
        Task<RegisterResponseDto> RegisterAsync(RegisterDto registerDto);
        Task<AuthResponseDto> LoginAsync(LoginDto loginDto);
        Task<bool> ActivateAccountAsync(VerifyOtpDto verifyOtpDto);
        Task<RegisterResponseDto> ResendOtpAsync(string email);
    }
}