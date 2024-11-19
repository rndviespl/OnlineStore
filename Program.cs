using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using System.Text;
using WebApp2.Controllers;
using WebApp2.Data;

namespace WebApp2
{
    public class Program
    {
        public static void Main(string[] args)
        {
            var builder = WebApplication.CreateBuilder(args);

            // –егистраци€ ApplicationContext с зависимост€ми
            builder.Services.AddDbContext<ApplicationContext>((serviceProvider, options) =>
            {
                var configuration = serviceProvider.GetRequiredService<IConfiguration>();
                var connectionString = configuration.GetConnectionString("Default");
                options.UseMySql(connectionString, Microsoft.EntityFrameworkCore.ServerVersion.Parse("8.0.39-mysql"));
            });
            builder.Services.AddHttpClient<BrosShopImagesController>();
            builder.Services.AddAuthentication(options =>
            {
                options.DefaultAuthenticateScheme = JwtBearerDefaults.AuthenticationScheme;
                options.DefaultChallengeScheme = JwtBearerDefaults.AuthenticationScheme;
            })
    .AddJwtBearer(options =>
    {
        options.TokenValidationParameters = new TokenValidationParameters
        {
            ValidateIssuer = true,
            ValidateAudience = true,
            ValidateLifetime = true,
            ValidateIssuerSigningKey = true,
            ValidIssuer = "your_issuer", // ”кажите ваш издатель
            ValidAudience = "your_audience", // ”кажите вашу аудиторию
            IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes("your_secret_key")) // ”кажите ваш секретный ключ
        };
    });

            builder.Services.AddControllersWithViews();
            // Add services to the container.
            builder.Services.AddControllersWithViews();

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

            app.UseAuthorization();

            app.MapControllerRoute(
                name: "default",
                pattern: "{controller=Home}/{action=Index}/{id?}");
            app.MapControllerRoute(
                 name: "login",
                 pattern: "BrosShopUsers/Login",
                 defaults: new { controller = "BrosShopUsers", action = "Login" });
            app.MapControllerRoute(
                name: "product",
                pattern: "{controller=BrosShopProducts}/{action=Index}/{id?}");
            app.MapControllerRoute(
                           name: "cart",
                           pattern: "{controller=Cart}/{action=Index}/{id?}");
            app.MapControllerRoute(
               name: "user",
               pattern: "{controller=BrosShopUser}/{action=Index}/{id?}");
            app.Run();
        }
    }
}
