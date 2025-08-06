using Microsoft.EntityFrameworkCore;
using UserManagement_API.Models.Entities;
using UserManagement_API.Repositories.IRepository;

namespace UserManagement_API.Repositories
{
    public class UserRepository : IUserRepository
    {
        private readonly AppDbContext _context;

        public UserRepository(AppDbContext context)
        {
            _context = context;
        }

        public async Task<User?> GetByUsernameAsync(string username)
        {
            return await _context.User
                .Include(u => u.Role)
                .FirstOrDefaultAsync(u => u.Username == username);
        }

        public async Task<User?> GetByEmailAsync(string email)
        {
            return await _context.User
                .Include(u => u.Role)
                .FirstOrDefaultAsync(u => u.Email == email);
        }

        public async Task<User> CreateAsync(User user)
        {
            _context.User.Add(user);
            await _context.SaveChangesAsync();
            return user;
        }

        public async Task<User> UpdateAsync(User user)
        {
            _context.User.Update(user);
            await _context.SaveChangesAsync();
            return user;
        }

        public async Task<bool> ExistsByUsernameAsync(string username)
        {
            return await _context.User.AnyAsync(u => u.Username == username);
        }

        public async Task<bool> ExistsByEmailAsync(string email)
        {
            return await _context.User.AnyAsync(u => u.Email == email);
        }

        public async Task<User?> GetByIdWithRoleAsync(int id)
        {
            return await _context.User
                .Include(u => u.Role)
                .FirstOrDefaultAsync(u => u.UserId == id);
        }

        public async Task<User?> GetByIdAsync(int id)
        {
            return await _context.User.FindAsync(id);
        }

        public async Task<IEnumerable<User>> GetAllUsersWithRoleAsync()
        {
            return await _context.User
                .Include(u => u.Role)
                .OrderBy(u => u.UserId)
                .ToListAsync();
        }

        public async Task<bool> DeleteAsync(int id)
        {
            var user = await _context.User.FindAsync(id);
            if (user == null)
                return false;

            _context.User.Remove(user);
            await _context.SaveChangesAsync();
            return true;
        }

        // OTP methods
        public async Task SaveOtpAsync(string email, string otp)
        {
            var user = await _context.User.FirstOrDefaultAsync(u => u.Email == email);
            if (user != null)
            {
                user.Otp = otp;
                user.OtpCreatedAt = DateTime.UtcNow;
                user.UpdatedAt = DateTime.UtcNow;
                await _context.SaveChangesAsync();
            }
        }

        public async Task<bool> VerifyOtpAsync(string email, string otp)
        {
            var user = await _context.User.FirstOrDefaultAsync(u => u.Email == email);
            if (user == null || user.Otp != otp)
                return false;

            if (user.OtpCreatedAt == null)
                return false;

            var elapsedTime = DateTime.UtcNow - user.OtpCreatedAt.Value;
            return elapsedTime.TotalMinutes <= 5;
        }

        public async Task<(string? OtpCode, DateTime? OtpCreatedAt)> GetOtpInfoAsync(string email)
        {
            var user = await _context.User.FirstOrDefaultAsync(u => u.Email == email);
            return user != null ? (user.Otp, user.OtpCreatedAt) : (null, null);
        }
    }
}