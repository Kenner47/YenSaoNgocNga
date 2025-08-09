using Microsoft.EntityFrameworkCore;
using ProductManagement_API.Data;
using ProductManagement_API.Repositories;
using ProductManagement_API.Repositories.IRepository;
using ProductManagement_API.Services;
using ProductManagement_API.Services.IService;

namespace ProductManagement_API.Injection
{
    public static class DependencyInjection
    {
        public static IServiceCollection AddProductServices(this IServiceCollection services, IConfiguration configuration)
        {
            // Database Context
            services.AddDbContext<AppDbContext>(options =>
                options.UseNpgsql(configuration.GetConnectionString("DefaultConnection")));

            // Repositories
            services.AddScoped<IProductRepository, ProductRepository>();
            services.AddScoped<ICategoryRepository, CategoryRepository>();
            services.AddScoped<IInventoryRepository, InventoryRepository>();

            // Services
            services.AddScoped<IProductService, ProductService>();
            services.AddScoped<ICategoryService, CategoryService>();
            services.AddScoped<IInventoryService, InventoryService>();

            return services;
        }
    }
}