using Microsoft.EntityFrameworkCore;
using UserManagement_API.Models.Entities;

public class AppDbContext : DbContext
{
    public AppDbContext(DbContextOptions<AppDbContext> options) : base(options) { }

    public DbSet<User> User { get; set; }
    public DbSet<Role> Role { get; set; }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        base.OnModelCreating(modelBuilder);

        // Seed Roles
        modelBuilder.Entity<Role>().HasData(
            new Role { RoleId = 1, RoleName = "Admin" },
            new Role { RoleId = 2, RoleName = "Employee" },
            new Role { RoleId = 3, RoleName = "User" }
        );

        // Seed sample Users
        modelBuilder.Entity<User>().HasData(
            new User
            {
                UserId = 1,
                Username = "admin",
                Password = "admin123", // Trong thực tế nên hash password
                FullName = "System Administrator",
                Email = "admin@example.com",
                PhoneNumber = "0123456789",
                Address = "System Address",
                DateOfBirth = new DateOnly(1990, 1, 1),
                Sex = "Male",
                RoleId = 1,
                IsActive = true,
                CreatedAt = new DateTime(2025, 8, 1, 0, 0, 0, DateTimeKind.Utc),
                UpdatedAt = new DateTime(2025, 8, 1, 0, 0, 0, DateTimeKind.Utc)
            },
            new User
            {
                UserId = 2,
                Username = "employee1",
                Password = "emp123",
                FullName = "Company Employee",
                Email = "employee1@example.com",
                PhoneNumber = "0555666777",
                Address = "Employee 1 Address",
                DateOfBirth = new DateOnly(1992, 8, 20),
                Sex = "Male",
                RoleId = 2,
                IsActive = true,
                CreatedAt = new DateTime(2025, 8, 1, 0, 0, 0, DateTimeKind.Utc),
                UpdatedAt = new DateTime(2025, 8, 1, 0, 0, 0, DateTimeKind.Utc)
            },
            new User
            {
                UserId = 3,
                Username = "employee2",
                Password = "emp123",
                FullName = "Regular Employee",
                Email = "employee2@example.com",
                PhoneNumber = "0987654321",
                Address = "Employee 2 Address",
                DateOfBirth = new DateOnly(1995, 5, 15),
                Sex = "Female",
                RoleId = 2,
                IsActive = true,
                CreatedAt = new DateTime(2025, 8, 1, 0, 0, 0, DateTimeKind.Utc),
                UpdatedAt = new DateTime(2025, 8, 1, 0, 0, 0, DateTimeKind.Utc)
            }
        );
    }
}