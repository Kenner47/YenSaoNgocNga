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

        public async Task<RoleDto> CreateRoleAsync(CreateRoleDto createRoleDto)
        {
            // Check if role name already exists
            if (await _roleRepository.RoleNameExistsAsync(createRoleDto.RoleName))
            {
                throw new InvalidOperationException($"Role name '{createRoleDto.RoleName}' already exists.");
            }

            var role = new Role
            {
                RoleName = createRoleDto.RoleName
            };

            var createdRole = await _roleRepository.CreateAsync(role);

            return new RoleDto
            {
                RoleId = createdRole.RoleId,
                RoleName = createdRole.RoleName
            };
        }

        public async Task<RoleDto?> UpdateRoleAsync(int id, UpdateRoleDto updateRoleDto)
        {
            // Check if role exists
            if (!await _roleRepository.ExistsAsync(id))
                return null;

            // Check if new role name already exists (excluding current role)
            if (await _roleRepository.RoleNameExistsAsync(updateRoleDto.RoleName, id))
            {
                throw new InvalidOperationException($"Role name '{updateRoleDto.RoleName}' already exists.");
            }

            var role = new Role
            {
                RoleName = updateRoleDto.RoleName
            };

            var updatedRole = await _roleRepository.UpdateAsync(id, role);
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
