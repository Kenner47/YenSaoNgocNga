using Microsoft.EntityFrameworkCore;
using PaymentManagement_API.Data;

namespace PaymentManagement_API.Injection
{
    public static class DependencyInjection
    {
        public static IServiceCollection AddPaymentServices(this IServiceCollection services, IConfiguration configuration)
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
