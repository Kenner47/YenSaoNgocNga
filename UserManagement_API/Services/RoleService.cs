using UserManagement_API.Models.DTOs;
using UserManagement_API.Models.Entities;
using UserManagement_API.Repositories.IRepository;
using UserManagement_API.Services.IService;

namespace UserManagement_API.Services
{
    public class RoleService : IRoleService
    {
        private readonly IRoleRepository _roleRepository;

        public RoleService(IRoleRepository roleRepository)
        {
            _roleRepository = roleRepository;
        }

        public async Task<IEnumerable<RoleDto>> GetAllRolesAsync()
        {
            var roles = await _roleRepository.GetAllAsync();
            return roles.Select(r => new RoleDto
            {
                RoleId = r.RoleId,
                RoleName = r.RoleName
            });
        }

        public async Task<RoleDto?> GetRoleByIdAsync(int id)
        {
            var role = await _roleRepository.GetByIdAsync(id);
            if (role == null)
                return null;

            return new RoleDto
            {
                RoleId = role.RoleId,
                RoleName = role.RoleName
            };
        }

        public async Task<RoleDto> CreateRoleAsync(RoleDto roleDto)
        {
            var role = new Role
            {
                RoleName = roleDto.RoleName
            };

            var createdRole = await _roleRepository.CreateAsync(role);

            return new RoleDto
            {
                RoleId = createdRole.RoleId,
                RoleName = createdRole.RoleName
            };
        }

        public async Task<RoleDto?> UpdateRoleAsync(int id, RoleDto roleDto)
        {
            var role = new Role
            {
                RoleId = id,
                RoleName = roleDto.RoleName
            };

            var updatedRole = await _roleRepository.UpdateAsync(role);
            if (updatedRole == null)
                return null;

            return new RoleDto
            {
                RoleId = updatedRole.RoleId,
                RoleName = updatedRole.RoleName
            };
        }

        public async Task<bool> DeleteRoleAsync(int id)
        {
            return await _roleRepository.DeleteAsync(id);
        }
    }
}
