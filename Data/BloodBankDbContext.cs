using Microsoft.EntityFrameworkCore;
using BloodBankSystem.Models;

namespace BloodBankSystem.Data
{
    public class BloodBankDbContext : DbContext
    {
        public BloodBankDbContext(DbContextOptions<BloodBankDbContext> options)
            : base(options) { }

        public DbSet<Recipient> Recipients { get; set; }
        public DbSet<BloodRequest> BloodRequests { get; set; }
    }
}
