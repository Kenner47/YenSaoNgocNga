using YenSaoNgocNga_MVC.Areas.UserManagement.Services;
using YenSaoNgocNga_MVC.Areas.ProductManagement.Services;

namespace YenSaoNgocNga_MVC
{
    public class Program
    {
        public static void Main(string[] args)
        {
            var builder = WebApplication.CreateBuilder(args);

            // Add services to the container.
            builder.Services.AddControllersWithViews();

            // Add HttpContextAccessor
            builder.Services.AddHttpContextAccessor();

            // Add Session support
            builder.Services.AddSession(options =>
            {
                options.IdleTimeout = TimeSpan.FromMinutes(30);
                options.Cookie.HttpOnly = true;
                options.Cookie.IsEssential = true;
            });

            // Add HttpClient for API communication
            builder.Services.AddHttpClient();

            // Add API Services
            builder.Services.AddScoped<IUserApiService, UserApiService>();
            builder.Services.AddScoped<IProductApiService, ProductApiService>();

            var app = builder.Build();

            // Configure the HTTP request pipeline.
            if (!app.Environment.IsDevelopment())
            {
                app.UseExceptionHandler("/Home/Error");
                app.UseHsts();
            }

            app.UseHttpsRedirection();
            app.UseStaticFiles();

            app.UseRouting();

            // Add Session middleware
            app.UseSession();

            app.UseAuthorization();

            // Areas routing - IMPORTANT!
            app.MapControllerRoute(
                name: "areas",
                pattern: "{area:exists}/{controller=Home}/{action=Index}/{id?}");

            // Default routing
            app.MapControllerRoute(
                name: "default",
                pattern: "{controller=Home}/{action=Index}/{id?}");

            app.Run();
        }
    }
}