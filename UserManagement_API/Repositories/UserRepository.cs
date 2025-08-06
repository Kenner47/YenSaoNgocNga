using Microsoft.EntityFrameworkCore;
using UserManagement_API.Models.DTOs;
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

        public async Task<IEnumerable<User>> SearchUsersAsync(string? keyword)
        {
            var query = _context.User.Include(u => u.Role).AsQueryable();

            if (!string.IsNullOrEmpty(keyword))
            {
                var searchTerm = keyword.ToLower();
                query = query.Where(u =>
                    u.Username.ToLower().Contains(searchTerm) ||
                    u.FullName.ToLower().Contains(searchTerm) ||
                    u.Email.ToLower().Contains(searchTerm));
            }

            return await query
                .OrderByDescending(u => u.CreatedAt)
                .ToListAsync();
        }

        public async Task<UserStatisticsDto> GetUserStatisticsAsync()
        {
            var now = DateTime.UtcNow;
            // 🔧 FIX: Đảm bảo tất cả DateTime đều có Kind = UTC
            var today = DateTime.SpecifyKind(now.Date, DateTimeKind.Utc);
            var weekStart = DateTime.SpecifyKind(today.AddDays(-(int)today.DayOfWeek), DateTimeKind.Utc);
            var monthStart = DateTime.SpecifyKind(new DateTime(now.Year, now.Month, 1), DateTimeKind.Utc);

            // 📊 Basic counts
            var totalUsers = await _context.User.CountAsync();
            var activeUsers = await _context.User.CountAsync(u => u.IsActive);
            var inactiveUsers = totalUsers - activeUsers;

            // 🎭 Users by Role
            var adminCount = await _context.User.CountAsync(u => u.RoleId == 1);
            var employeeCount = await _context.User.CountAsync(u => u.RoleId == 2);
            var userCount = await _context.User.CountAsync(u => u.RoleId == 3);

            // 👫 Users by Gender
            var maleCount = await _context.User.CountAsync(u => u.Sex.ToLower() == "male" || u.Sex.ToLower() == "nam");
            var femaleCount = await _context.User.CountAsync(u => u.Sex.ToLower() == "female" || u.Sex.ToLower() == "nữ" || u.Sex.ToLower() == "nu");
            var otherCount = totalUsers - maleCount - femaleCount;

            // 📅 Recent registrations - 🔧 FIX: So sánh với >= thay vì ==
            var todayRegistrations = await _context.User.CountAsync(u => u.CreatedAt >= today);
            var thisWeekRegistrations = await _context.User.CountAsync(u => u.CreatedAt >= weekStart);
            var thisMonthRegistrations = await _context.User.CountAsync(u => u.CreatedAt >= monthStart);

            return new UserStatisticsDto
            {
                TotalUsers = totalUsers,
                ActiveUsers = activeUsers,
                InactiveUsers = inactiveUsers,
                UsersByRole = new UsersByRoleDto
                {
                    AdminCount = adminCount,
                    EmployeeCount = employeeCount,
                    UserCount = userCount
                },
                UsersByGender = new UsersByGenderDto
                {
                    MaleCount = maleCount,
                    FemaleCount = femaleCount,
                    OtherCount = otherCount
                },
                RecentUsers = new RecentUsersDto
                {
                    TodayRegistrations = todayRegistrations,
                    ThisWeekRegistrations = thisWeekRegistrations,
                    ThisMonthRegistrations = thisMonthRegistrations
                }
            };
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