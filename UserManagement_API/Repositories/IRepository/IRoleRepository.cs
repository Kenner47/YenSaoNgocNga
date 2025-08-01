using UserManagement_API.Models.Entities;

namespace UserManagement_API.Repositories.IRepository
{
    public interface IRoleRepository
    {
        Task<IEnumerable<Role>> GetAllAsync();
        Task<Role?> GetByIdAsync(int id);
        Task<Role> CreateAsync(Role role);
        Task<Role?> UpdateAsync(int id, Role role);
        Task<bool> DeleteAsync(int id);
        Task<bool> ExistsAsync(int id);
        Task<bool> RoleNameExistsAsync(string roleName, int? excludeId = null);

    }
}
