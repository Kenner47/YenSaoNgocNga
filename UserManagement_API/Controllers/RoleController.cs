using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using UserManagement_API.Models.DTOs;
using UserManagement_API.Services.IService;

namespace UserManagement_API.Controllers
{
    [Authorize(Roles = "Admin")]
    [ApiController]
    [Route("api/[controller]")]
    public class RoleController : ControllerBase
    {
        private readonly IRoleService _roleService;

        public RoleController(IRoleService roleService)
        {
            _roleService = roleService;
        }

        [HttpGet]
        public async Task<ActionResult<IEnumerable<RoleDto>>> GetAllRoles()
        {
            var roles = await _roleService.GetAllRolesAsync();
            return Ok(roles);
        }

        [HttpGet("{id}")]
        public async Task<ActionResult<RoleDto>> GetRole(int id)
        {
            var role = await _roleService.GetRoleByIdAsync(id);
            if (role == null)
                return NotFound($"Role with ID {id} not found.");

            return Ok(role);
        }

        [HttpPost]
        public async Task<ActionResult<RoleDto>> CreateRole(RoleDto roleDto)
        {
            if (!ModelState.IsValid)
                return BadRequest(ModelState);

            var createdRole = await _roleService.CreateRoleAsync(roleDto);
            return CreatedAtAction(nameof(GetRole), new { id = createdRole.RoleId }, createdRole);
        }

        [HttpPut("{id}")]
        public async Task<ActionResult<RoleDto>> UpdateRole(int id, RoleDto roleDto)
        {
            if (!ModelState.IsValid)
                return BadRequest(ModelState);

            var updatedRole = await _roleService.UpdateRoleAsync(id, roleDto);
            if (updatedRole == null)
                return NotFound($"Role with ID {id} not found.");

            return Ok(updatedRole);
        }

        [HttpDelete("{id}")]
        public async Task<ActionResult> DeleteRole(int id)
        {
            var result = await _roleService.DeleteRoleAsync(id);
            if (!result)
                return NotFound($"Role with ID {id} not found.");

            return NoContent();
        }
    }
}