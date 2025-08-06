using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using UserManagement_API.Models.DTOs;
using UserManagement_API.Services.IService;

namespace UserManagement_API.Controllers
{
    [Authorize(Roles = "Admin")]
    [ApiController]
    [Route("api/[controller]")]
    public class AdminController : ControllerBase
    {
        private readonly IUserService _userService;

        public AdminController(IUserService userService)
        {
            _userService = userService;
        }

        [HttpGet("users")]
        public async Task<ActionResult<IEnumerable<UserDto>>> GetAllUsers()
        {
            var users = await _userService.GetAllUsersAsync();
            return Ok(users);
        }

        [HttpGet("users/{id}")]
        public async Task<ActionResult<UserDto>> GetUser(int id)
        {
            var user = await _userService.GetUserByIdAsync(id);
            if (user == null)
                return NotFound($"User with ID {id} not found.");

            return Ok(user);
        }

        [HttpPut("users/{id}")]
        public async Task<ActionResult<UserDto>> UpdateUser(int id, AdminUpdateUserDto updateDto)
        {
            if (!ModelState.IsValid)
                return BadRequest(ModelState);

            var updatedUser = await _userService.UpdateUserAsync(id, updateDto);
            if (updatedUser == null)
                return NotFound($"User with ID {id} not found.");

            return Ok(updatedUser);
        }

        [HttpPut("users/{id}/toggle-status")]
        public async Task<ActionResult> ToggleUserStatus(int id)
        {
            var result = await _userService.ToggleUserStatusAsync(id);
            if (!result)
                return NotFound($"User with ID {id} not found.");

            return Ok(new { message = "User status updated successfully." });
        }

        [HttpDelete("users/{id}")]
        public async Task<ActionResult> DeleteUser(int id)
        {
            var result = await _userService.DeleteUserAsync(id);
            if (!result)
                return NotFound($"User with ID {id} not found.");

            return Ok(new { message = "User deactivated successfully." });
        }
    }
}