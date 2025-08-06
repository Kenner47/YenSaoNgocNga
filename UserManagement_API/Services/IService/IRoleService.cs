using UserManagement_API.Models.DTOs;

namespace UserManagement_API.Services.IService
{
    public interface IRoleService
    {
        Task<IEnumerable<RoleDto>> GetAllRolesAsync();
        Task<RoleDto?> GetRoleByIdAsync(int id);
        Task<RoleDto> CreateRoleAsync(RoleDto roleDto);
        Task<RoleDto?> UpdateRoleAsync(int id, RoleDto roleDto);
        Task<bool> DeleteRoleAsync(int id);
    }
}
