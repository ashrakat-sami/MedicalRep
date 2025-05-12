using Microsoft.AspNetCore.Identity;
using MedicalRep.Models;
using Microsoft.EntityFrameworkCore;
using MedicalRep.Hubs;
using Microsoft.AspNetCore.Identity.UI.Services;

namespace MedicalRep
{
    public class Program
    {
        public static async Task Main(string[] args)
        {
            var builder = WebApplication.CreateBuilder(args);

            // Add services to the container.
            builder.Services.AddControllersWithViews();

         

            // Add DbContext
            builder.Services.AddDbContext<ApplicationDbContext>(options =>
                options.UseSqlServer(builder.Configuration.GetConnectionString("DefaultConnection")));

            // Add Identity
            builder.Services.AddIdentity<ApplicationUser, IdentityRole>(options =>
            {
                options.SignIn.RequireConfirmedAccount = false;
                options.Password.RequireDigit = true;
                options.Password.RequiredLength = 6;
                options.Password.RequireNonAlphanumeric = false;
                options.Password.RequireUppercase = true;
                options.Password.RequireLowercase = true;
            })
            .AddEntityFrameworkStores<ApplicationDbContext>()
            .AddDefaultTokenProviders();

            // Add email configuration
            builder.Services.Configure<EmailSettings>(builder.Configuration.GetSection("EmailSettings"));
            builder.Services.AddTransient<IEmailSender, EmailSender>();

            // Add Razor Pages support
            builder.Services.AddRazorPages();

            // Add SignalR services
            builder.Services.AddSignalR();
            builder.Services.AddAuthorization(options =>
            {
                options.AddPolicy("RequireMedicalRepRole", policy => policy.RequireRole("MedicalRep"));
                options.AddPolicy("RequireDoctorRole", policy => policy.RequireRole("Doctor"));
                options.AddPolicy("RequireAdminRole", policy => policy.RequireRole("Admin"));
            });
            // Configure Identity options
            builder.Services.ConfigureApplicationCookie(options =>
            {
                options.Cookie.HttpOnly = true;
                options.ExpireTimeSpan = TimeSpan.FromMinutes(60);
                options.LoginPath = "/Account/Login";
                options.AccessDeniedPath = "/Account/AccessDenied";
                options.SlidingExpiration = true;
            });



            var app = builder.Build();

            //seeed data by using service provider 
            using (var scope = app.Services.CreateScope())
            {
                var services = scope.ServiceProvider;
                await SeedData.InitializeAsync(services);

            }


            // Configure the HTTP request pipeline.
            if (!app.Environment.IsDevelopment())
            {
                app.UseExceptionHandler("/Home/Error");
                app.UseHsts();
            }
            // Configure the HTTP request pipeline
            app.MapHub<NotificationHub>("/notificationHub");

            app.UseHttpsRedirection();
            app.UseStaticFiles();

            app.UseRouting();

            app.UseAuthentication();
            app.UseAuthorization();

            app.MapControllerRoute(
                name: "default",
                pattern: "{controller=Account}/{action=Login}/{id?}");

            app.MapRazorPages();

            app.Run();
        }
    }
}