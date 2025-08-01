using Microsoft.EntityFrameworkCore;
using UserManagement_API.Models.Entities;
using UserManagement_API.Repositories.IRepository;

namespace UserManagement_API.Repositories
{
    public class RoleRepository : IRoleRepository
    {
        private readonly AppDbContext _context;

        public RoleRepository(AppDbContext context)
        {
            _context = context;
        }

        public async Task<IEnumerable<Role>> GetAllAsync()
        {
            return await _context.Role.ToListAsync();
        }

        public async Task<Role?> GetByIdAsync(int id)
        {
            return await _context.Role.FindAsync(id);
        }

        public async Task<Role> CreateAsync(Role role)
        {
            _context.Role.Add(role);
            await _context.SaveChangesAsync();
            return role;
        }

        public async Task<Role?> UpdateAsync(int id, Role role)
        {
            var existingRole = await _context.Role.FindAsync(id);
            if (existingRole == null)
                return null;

            existingRole.RoleName = role.RoleName;

            _context.Role.Update(existingRole);
            await _context.SaveChangesAsync();
            return existingRole;
        }

        public async Task<bool> DeleteAsync(int id)
        {
            var role = await _context.Role.FindAsync(id);
            if (role == null)
                return false;

            _context.Role.Remove(role);
            await _context.SaveChangesAsync();
            return true;
        }

        public async Task<bool> ExistsAsync(int id)
        {
            return await _context.Role.AnyAsync(r => r.RoleId == id);
        }

        public async Task<bool> RoleNameExistsAsync(string roleName, int? excludeId = null)
        {
            return await _context.Role.AnyAsync(r => r.RoleName == roleName && (excludeId == null || r.RoleId != excludeId));
        }
    }
}
