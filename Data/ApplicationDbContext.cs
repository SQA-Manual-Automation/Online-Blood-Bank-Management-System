using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;
using BloodBankSystem.Models;

namespace BloodBankSystem.Data
{
    public class ApplicationDbContext : IdentityDbContext<ApplicationUser>
    {
        public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options)
            : base(options)
        {
        }

        public DbSet<Recipient> Recipients { get; set; } = default!;
        public DbSet<Donor> Donors { get; set; } = default!;
        public DbSet<BloodRequest> BloodRequests { get; set; } = default!;
        public DbSet<Admin> Admins { get; set; } = default!;

        protected override void OnModelCreating(ModelBuilder builder)
        {
            base.OnModelCreating(builder);

            builder.Entity<Admin>().HasData(new Admin
            {
                AdminId = 99,
                Username = "Admin",
                Password = "123456"
            });
        }
    }
}
