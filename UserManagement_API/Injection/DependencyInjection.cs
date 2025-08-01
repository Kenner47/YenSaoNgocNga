using Microsoft.EntityFrameworkCore;
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

            // Register repositories
            services.AddScoped<IRoleRepository, RoleRepository>();

            // Register services
            services.AddScoped<IRoleService, RoleService>();


            return services;
        }
    }
}
