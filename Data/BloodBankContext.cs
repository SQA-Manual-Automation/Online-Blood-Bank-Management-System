using Microsoft.EntityFrameworkCore;
using BloodBankSystem.Models;

namespace BloodBankSystem.Data
{
    public class BloodBankContext : DbContext
    {
        public BloodBankContext(DbContextOptions<BloodBankContext> options) : base(options) { }

        public DbSet<Donor> Donors { get; set; }
        public DbSet<Recipient> Recipients { get; set; }
        public DbSet<BloodInventory> BloodInventories { get; set; }
        public DbSet<BloodRequest> BloodRequests { get; set; }
        public DbSet<HospitalRepresentative> HospitalRepresentatives { get; set; }
    }
}
