using Microsoft.AspNetCore.Mvc;
using UserManagement_API.Models.DTOs;
using UserManagement_API.Services.IService;

namespace UserManagement_API.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class AuthController : ControllerBase
    {
        private readonly IAuthService _authService;

        public AuthController(IAuthService authService)
        {
            _authService = authService;
        }

        [HttpPost("register")]
        public async Task<ActionResult<RegisterResponseDto>> Register(RegisterDto registerDto)
        {
            if (!ModelState.IsValid)
                return BadRequest(ModelState);

            var result = await _authService.RegisterAsync(registerDto);

            if (!result.Success)
                return BadRequest(result);

            return Ok(result);
        }

        [HttpPost("activate-account")]
        public async Task<IActionResult> ActivateAccount([FromBody] VerifyOtpDto otpModel)
        {
            if (otpModel == null)
                return BadRequest("Invalid OTP details");

            var result = await _authService.ActivateAccountAsync(otpModel);
            if (!result)
                return BadRequest("Account activation failed");

            return Ok("Account activated successfully");
        }

        [HttpPost("resend-otp")]
        public async Task<IActionResult> ResendOtp([FromBody] ResendOtpDto model)
        {
            if (string.IsNullOrEmpty(model.Email))
            {
                return BadRequest("Email is required.");
            }

            var response = await _authService.ResendOtpAsync(model.Email);
            if (response.Success)
            {
                return Ok(response.Message);
            }

            return BadRequest(response.Message);
        }

        [HttpPost("login")]
        public async Task<ActionResult<AuthResponseDto>> Login(LoginDto loginDto)
        {
            if (!ModelState.IsValid)
                return BadRequest(ModelState);

            var result = await _authService.LoginAsync(loginDto);

            if (result.UserId == 0)
                return Unauthorized(result);

            return Ok(result);
        }
    }
}