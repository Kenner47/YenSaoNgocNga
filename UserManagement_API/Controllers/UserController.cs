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

        public UserController(IUserService userService)
        {
            _userService = userService;
        }

        [HttpGet("profile/{userId}")]
        public async Task<ActionResult<UserDto>> GetMyProfile(int userId)
        {
            var user = await _userService.GetMyProfileAsync(userId);
            if (user == null)
                return NotFound("User not found.");

            return Ok(user);
        }

        [HttpPut("profile/{userId}")]
        public async Task<ActionResult<UserDto>> UpdateMyProfile(int userId, UpdateUserDto updateDto)
        {
            if (!ModelState.IsValid)
                return BadRequest(ModelState);

            var updatedUser = await _userService.UpdateMyProfileAsync(userId, updateDto);
            if (updatedUser == null)
                return NotFound("User not found.");

            return Ok(updatedUser);
        }

        [HttpPut("profile/{userId}/change-password")]
        public async Task<ActionResult> ChangePassword(int userId, ChangePasswordDto changePasswordDto)
        {
            if (!ModelState.IsValid)
                return BadRequest(ModelState);

            var result = await _userService.ChangePasswordAsync(userId, changePasswordDto);
            if (!result)
                return BadRequest("Failed to change password. Please check your current password.");

            return Ok(new { message = "Password changed successfully." });
        }
    }
}