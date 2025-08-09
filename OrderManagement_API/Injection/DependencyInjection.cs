using Microsoft.EntityFrameworkCore;
using OrderManagement_API.Data;

namespace OrderManagement_API.Injection
{
    public static class DependencyInjection
    {
        public static IServiceCollection AddOrderServices(this IServiceCollection services, IConfiguration configuration)
        {
            // Database Context
            services.AddDbContext<AppDbContext>(options =>
                options.UseNpgsql(configuration.GetConnectionString("DefaultConnection")));

            // Repositories
            

            // Services

            return services;
        }
    }
}