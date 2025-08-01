using UserManagement_API.Models.DTOs;

namespace UserManagement_API.Services.IService
{
    public interface IAuthService
    {
        Task<AuthResponseDto> RegisterAsync(RegisterDto registerDto);
        Task<AuthResponseDto> LoginAsync(LoginDto loginDto);

    }
}
