using Microsoft.EntityFrameworkCore;
using UserManagement_API.Helpers;
using UserManagement_API.Repositories;
using UserManagement_API.Repositories.IRepository;
using UserManagement_API.Services;
using UserManagement_API.Services.IService;

namespace UserManagement_API.Injection
{
    public static class DependencyInjection
    {
        public static IServiceCollection AddUserServices(this IServiceCollection services, IConfiguration configuration)
        {
            services.AddDbContext<AppDbContext>(options =>
                options.UseNpgsql(configuration.GetConnectionString("DefaultConnection")));

            // PasswordHelper
            services.AddScoped<PasswordHelper>();

            // JWT-related
            services.AddScoped<ICurrentUserService, CurrentUserService>();
            services.AddHttpContextAccessor();

            // Register repositories
            services.AddScoped<IRoleRepository, RoleRepository>();
            services.AddScoped<IUserRepository, UserRepository>();


            // Register services
            services.AddScoped<IRoleService, RoleService>();
            services.AddScoped<IAuthService, AuthService>();
            services.AddScoped<IEmailService, EmailService>();
            services.AddScoped<IUserService, UserService>();


            return services;
        }
    }
}
