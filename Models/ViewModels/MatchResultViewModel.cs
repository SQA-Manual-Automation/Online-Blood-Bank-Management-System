using System.Collections.Generic;
using BloodBankSystem.Models;

namespace BloodBankSystem.Models.ViewModels
{
    public class MatchResultViewModel
    {
        public string RecipientName { get; set; } = string.Empty;
        public string RequestedBloodGroup { get; set; } = string.Empty;
        public List<Donor> MatchingDonors { get; set; } = new List<Donor>();
    }
}
