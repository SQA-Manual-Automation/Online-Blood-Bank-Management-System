namespace BloodBankSystem.Models
{
    public class BloodInventory
    {
        public int BloodInventoryId { get; set; }
        public string? BloodType { get; set; }
        public int UnitsAvailable { get; set; }
        public DateTime LastUpdated { get; set; }
    }
}
