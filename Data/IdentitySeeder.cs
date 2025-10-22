using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.DependencyInjection;
using BloodBankSystem.Models;

namespace BloodBankSystem.Data
{
    public static class IdentitySeeder
    {
        public static async Task SeedAdminUserAsync(IServiceProvider services)
        {
            Console.WriteLine("IdentitySeeder.SeedAdminUserAsync() was triggered.");

            var userManager = services.GetRequiredService<UserManager<ApplicationUser>>();
            var roleManager = services.GetRequiredService<RoleManager<IdentityRole>>();

            var adminEmail = "admin@bloodbank.com";
            var adminPassword = "Admin123"; //  Pass

            // Create role if missing
            if (!await roleManager.RoleExistsAsync("Admin"))
            {
                await roleManager.CreateAsync(new IdentityRole("Admin"));
            }

            // Delete existing admin if present
            var existingAdmin = await userManager.FindByEmailAsync(adminEmail);
            if (existingAdmin != null)
            {
                await userManager.DeleteAsync(existingAdmin);
            }

            // Create new admin user
            var adminUser = new ApplicationUser
            {
                UserName = adminEmail,
                Email = adminEmail,
                EmailConfirmed = true
            };

            var result = await userManager.CreateAsync(adminUser, adminPassword);
            if (result.Succeeded)
            {
                await userManager.AddToRoleAsync(adminUser, "Admin");
                Console.WriteLine("Admin user seeded successfully.");
            }
            else
            {
                Console.WriteLine("Failed to seed admin user. Reasons:");
                foreach (var error in result.Errors)
                {
                    Console.WriteLine($"- {error.Code}: {error.Description}");
                }
            }
        }
    }
}
