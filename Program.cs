
using Microsoft.EntityFrameworkCore;
using WebApp2.Controllers;
using WebApp2.Data;

namespace WebApp2
{
    public class Program
    {
        public static void Main(string[] args)
        {
            IConfiguration _configuration = new ConfigurationBuilder()
                .SetBasePath(Directory.GetCurrentDirectory())
                .AddJsonFile("appsettings.json")
                .Build();

            var apiString = _configuration["ApiSettings:AuthUrl"];

            var builder = WebApplication.CreateBuilder(args);

            builder.Services.AddHttpClient<AuthService>(client =>
            {
                client.BaseAddress = new Uri(apiString); // ������� ������� ����� ������ API
            });
            builder.Services.AddHttpClient<BrosShopImagesController>();

            // ����������� ApplicationContext � �������������
            builder.Services.AddDbContextPool<ApplicationContext>((serviceProvider, options) =>
            {
                var configuration = serviceProvider.GetRequiredService<IConfiguration>();
                var connectionString = configuration.GetConnectionString("Default");
                options.UseMySql(connectionString, Microsoft.EntityFrameworkCore.ServerVersion.Parse("8.0.39-mysql"));
            });

            builder.Services.AddControllersWithViews();

            builder.Services.AddSession(options =>
            {
                options.IdleTimeout = TimeSpan.FromMinutes(30); // ���������� ����� ��������
                options.Cookie.HttpOnly = true; // �������� ����
                options.Cookie.IsEssential = true; // �������� ���� �������������
            });

            var app = builder.Build();

            // Configure the HTTP request pipeline.
            if (!app.Environment.IsDevelopment())
            {
                app.UseExceptionHandler("/Home/Error");
                // The default HSTS value is 30 days. You may want to change this for production scenarios, see https://aka.ms/aspnetcore-hsts.
                app.UseHsts();
            }

            app.UseHttpsRedirection();
            app.UseStaticFiles();

            app.UseRouting();

            app.UseSession();
            app.UseAuthentication();
            app.UseAuthorization();

            app.MapControllerRoute(
                name: "product",
                pattern: "{controller=BrosShopProducts}/{action=Index}/{id?}");
            app.MapControllerRoute(
                name: "default",
                pattern: "{controller=Home}/{action=Index}/{id?}");
            app.MapControllerRoute(
                 name: "login",
                 pattern: "BrosShopUsers/Login",
                 defaults: new { controller = "BrosShopUsers", action = "Login" });
            app.MapControllerRoute(
                           name: "cart",
                           pattern: "{controller=BrosShopCart}/{action=Index}/{id?}");
            app.MapControllerRoute(
               name: "user",
               pattern: "{controller=BrosShopUser}/{action=Index}/{id?}");
            app.Run();
        }
    }
}
