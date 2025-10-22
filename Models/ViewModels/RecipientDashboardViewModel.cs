using System.Collections.Generic;
using BloodBankSystem.Models;

namespace BloodBankSystem.Models.ViewModels
{
    public class RecipientDashboardViewModel
    {
        public string RecipientName { get; set; } = string.Empty;

        public int RecipientId { get; set; }

        public List<BloodRequest> RequestHistory { get; set; } = new List<BloodRequest>();

        public int TotalRequests { get; set; }

        public int PendingRequests { get; set; }

        public int ApprovedRequests { get; set; }
    }
}
