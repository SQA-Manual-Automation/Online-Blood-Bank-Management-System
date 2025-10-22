using Microsoft.EntityFrameworkCore;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Identity.UI.Services;
using BloodBankSystem.Data;
using BloodBankSystem.Data.Factories;
using BloodBankSystem.Data.Abstractions;
using BloodBankSystem.Data.Repositories;
using BloodBankSystem.Models;

public class Program
{
    public static async Task Main(string[] args)
    {
        var builder = WebApplication.CreateBuilder(args);

        // ✅ Register ApplicationDbContext
        builder.Services.AddDbContext<ApplicationDbContext>(options =>
        {
            options.UseSqlServer(builder.Configuration.GetConnectionString("DefaultConnection"));
            options.EnableSensitiveDataLogging();
            options.LogTo(Console.WriteLine, LogLevel.Information);
        });

        // ✅ Register BloodBankDbContext (used in RegisterModel)
        builder.Services.AddDbContext<BloodBankDbContext>(options =>
        {
            options.UseSqlServer(builder.Configuration.GetConnectionString("DefaultConnection"));
        });

        // ✅ Configure Identity
        builder.Services.AddIdentity<ApplicationUser, IdentityRole>(options =>
        {
            options.SignIn.RequireConfirmedAccount = false; // Email confirmation disabled
            options.Password.RequireDigit = true;
            options.Password.RequireLowercase = true;
            options.Password.RequireUppercase = true;
            options.Password.RequireNonAlphanumeric = false;
            options.Password.RequiredLength = 6;
        })
        .AddEntityFrameworkStores<ApplicationDbContext>()
        .AddDefaultTokenProviders();

        // ✅ Register DummyEmailSender to prevent registration crash
        builder.Services.AddTransient<IEmailSender, DummyEmailSender>();

        // ✅ Configure cookie paths
        builder.Services.ConfigureApplicationCookie(options =>
        {
            options.LoginPath = "/Identity/Account/Login";
            options.LogoutPath = "/Identity/Account/Logout";
            options.AccessDeniedPath = "/Identity/Account/AccessDenied";
            options.SlidingExpiration = true;
            options.ExpireTimeSpan = TimeSpan.FromMinutes(30);
        });

        builder.Services.AddControllersWithViews();
        builder.Services.AddRazorPages(options =>
        {
            options.Conventions.AddAreaPageRoute("Identity", "/Account/Login", "/Account/Login");
        });

        builder.Services.AddSession(options =>
        {
            options.IdleTimeout = TimeSpan.FromMinutes(30);
            options.Cookie.HttpOnly = true;
            options.Cookie.IsEssential = true;
        });

        builder.Services.AddScoped<IDbConnectionFactory, SqlConnectionFactory>();
        builder.Services.AddScoped<IRecipientRepository, RecipientRepository>();

        var app = builder.Build();

        Console.WriteLine(" App built. Seeding should start now.");

        using (var scope = app.Services.CreateScope())
        {
            var services = scope.ServiceProvider;

            try
            {
                await IdentitySeeder.SeedAdminUserAsync(services);
            }
            catch (Exception ex)
            {
                Console.WriteLine($" Seeding failed: {ex.Message}");
            }
        }

        if (!app.Environment.IsDevelopment())
        {
            app.UseExceptionHandler("/Home/Error");
            app.UseHsts();
        }

        // app.UseHttpsRedirection(); // if using HTTPS
        app.UseStaticFiles();

        app.UseRouting();

        app.UseSession();
        app.UseAuthentication();
        app.UseAuthorization();

        app.MapControllerRoute(
            name: "default",
            pattern: "{controller=Home}/{action=Index}/{id?}");

        app.MapRazorPages();

        app.Run();
    }
}
