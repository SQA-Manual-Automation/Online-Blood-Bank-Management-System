using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using BloodBankSystem.Models;

namespace BloodBankSystem.Controllers
{
    public class SeedController : Controller
    {
        private readonly UserManager<ApplicationUser> _userManager;
        private readonly RoleManager<IdentityRole> _roleManager;

        public SeedController(UserManager<ApplicationUser> userManager, RoleManager<IdentityRole> roleManager)
        {
            _userManager = userManager;
            _roleManager = roleManager;
        }

        public async Task<IActionResult> Index()
        {
            var adminEmail = "admin@bloodbank.com";
            var adminPassword = "admin123";

            if (!await _roleManager.RoleExistsAsync("Admin"))
            {
                await _roleManager.CreateAsync(new IdentityRole("Admin"));
            }

            var existingAdmin = await _userManager.FindByEmailAsync(adminEmail);
            if (existingAdmin != null)
            {
                await _userManager.DeleteAsync(existingAdmin);
            }

            var adminUser = new ApplicationUser
            {
                UserName = adminEmail,
                Email = adminEmail,
                EmailConfirmed = true
            };

            var result = await _userManager.CreateAsync(adminUser, adminPassword);
            if (result.Succeeded)
            {
                await _userManager.AddToRoleAsync(adminUser, "Admin");
                return Content("Admin user seeded successfully.");
            }

            return Content("Failed to seed admin user.");
        }
    }
}
