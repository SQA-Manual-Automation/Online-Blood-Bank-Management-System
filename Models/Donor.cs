using System;
using System.ComponentModel.DataAnnotations;

namespace BloodBankSystem.Models
{
    public class Donor
    {
        [Key]
        public int DonorId { get; set; } 

        [Required(ErrorMessage = "Full name is required")]
        [StringLength(100)]
        [Display(Name = "Full Name")]
        public string FullName { get; set; } = string.Empty;

        [Required(ErrorMessage = "Blood group is required")]
        [Display(Name = "Blood Group")]
        public string BloodGroup { get; set; } = string.Empty;

        [Required(ErrorMessage = "Contact number is required")]
        [Phone]
        [Display(Name = "Contact Number")]
        public string ContactNumber { get; set; } = string.Empty;

        [Required(ErrorMessage = "Location is required")]
        [Display(Name = "Location")]
        public string Location { get; set; } = string.Empty;

        [EmailAddress]
        [Display(Name = "Email Address")]
        public string? Email { get; set; } 

        [Display(Name = "Last Donation Date")]
        [DataType(DataType.Date)]
        public DateTime? LastDonationDate { get; set; }

        [Required(ErrorMessage = "Date of birth is required")]
        [DataType(DataType.Date)]
        [Display(Name = "Date of Birth")]
        public DateTime DateOfBirth { get; set; }

        [Display(Name = "Is Eligible")]
        public bool IsEligible { get; set; } = true;

        [Required(ErrorMessage = "User ID is required")]
        [Display(Name = "Application User ID")]
        public string ApplicationUserId { get; set; } = string.Empty;
        public int Id => DonorId;
        public string Name => FullName;
        public DateTime stDate => LastDonationDate ?? DateTime.MinValue;
        public string Password { get; set; } = string.Empty;

    }
}
