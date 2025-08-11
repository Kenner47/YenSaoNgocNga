using Microsoft.EntityFrameworkCore;
using PaymentManagement_API.Data;
using PaymentManagement_API.Models.Configurations;
using PaymentManagement_API.Repositories;
using PaymentManagement_API.Repositories.IRepository;
using PaymentManagement_API.Services;
using PaymentManagement_API.Services.IService;

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
            services.AddScoped<ITransactionRepository, TransactionRepository>();


            // Services
            services.Configure<VnPayConfig>(configuration.GetSection("VnPay"));
            services.AddScoped<ITransactionService, TransactionService>();
            services.AddScoped<IVnPayService, VnPayService>();


            return services;
        }
    }
}
