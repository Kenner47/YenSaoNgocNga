using UserManagement_API.Models.Entities;

namespace UserManagement_API.Repositories.IRepository
{
    public interface IRoleRepository
    {
        Task<IEnumerable<Role>> GetAllAsync();
        Task<Role?> GetByIdAsync(int id);
        Task<Role> CreateAsync(Role role);
        Task<Role?> UpdateAsync(Role role);
        Task<bool> DeleteAsync(int id);
        Task<bool> ExistsAsync(int id);
    }
}
