using Microsoft.EntityFrameworkCore;
using OrderManagement_API.Data;
using OrderManagement_API.Repositories;
using OrderManagement_API.Repositories.IRepository;
using OrderManagement_API.Services;
using OrderManagement_API.Services.IService;

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
            services.AddScoped<IOrderRepository, OrderRepository>();
            services.AddScoped<IOrderDetailRepository, OrderDetailRepository>();

            // Services
            services.AddScoped<IOrderService, OrderService>();
            services.AddScoped<IOrderDetailService, OrderDetailService>();

            return services;
        }
    }
}