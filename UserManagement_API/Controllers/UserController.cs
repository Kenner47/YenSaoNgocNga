using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using UserManagement_API.Models.DTOs;
using UserManagement_API.Services.IService;

namespace UserManagement_API.Controllers
{
    [Authorize]
    [ApiController]
    [Route("api/[controller]")]
    public class UserController : ControllerBase
    {
        private readonly IUserService _userService;
        private readonly ICurrentUserService _currentUserService; 

        public UserController(IUserService userService, ICurrentUserService currentUserService)
        {
            _userService = userService;
            _currentUserService = currentUserService;
        }

        [HttpGet("profile")]
        public async Task<ActionResult<UserDto>> GetMyProfile()
        {
            var currentUserId = _currentUserService.GetCurrentUserId();
            if (currentUserId == -1)
                return Unauthorized("Invalid token");

            var user = await _userService.GetMyProfileAsync(currentUserId);
            if (user == null)
                return NotFound("User not found.");

            return Ok(user);
        }

        [HttpPut("profile")]
        public async Task<ActionResult<UserDto>> UpdateMyProfile([FromBody] UpdateUserDto updateDto)
        {
            if (!ModelState.IsValid)
                return BadRequest(ModelState);

            var currentUserId = _currentUserService.GetCurrentUserId();
            if (currentUserId == -1)
                return Unauthorized("Invalid token");

            var updatedUser = await _userService.UpdateMyProfileAsync(currentUserId, updateDto);
            if (updatedUser == null)
                return NotFound("User not found.");

            return Ok(updatedUser);
        }

        [HttpPut("profile/change-password")]
        public async Task<ActionResult> ChangePassword([FromBody] ChangePasswordDto changePasswordDto)
        {
            if (!ModelState.IsValid)
                return BadRequest(ModelState);

            var currentUserId = _currentUserService.GetCurrentUserId();
            if (currentUserId == -1)
                return Unauthorized("Invalid token");

            var result = await _userService.ChangePasswordAsync(currentUserId, changePasswordDto);
            if (!result)
                return BadRequest("Failed to change password. Please check your current password.");

            return Ok(new { message = "Password changed successfully." });
        }
    }
}