using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace BloodBankSystem.Models
{
    public class Recipient
    {
        [Key]
        public int RecipientId { get; set; }

        [Required(ErrorMessage = "Full name is required")]
        [StringLength(100)]
        [Display(Name = "Full Name")]
        public string FullName { get; set; } = string.Empty;

        [Required(ErrorMessage = "Blood group is required")]
        [Display(Name = "Blood Group")]
        public string BloodGroup { get; set; } = string.Empty;

        [Display(Name = "Gender")]
        public string? Gender { get; set; }

        [DataType(DataType.Date)]
        [Display(Name = "Date of Birth")]
        public DateTime? DateOfBirth { get; set; }

        [Phone]
        [Display(Name = "Contact Number")]
        public string? ContactNumber { get; set; }

        [Required(ErrorMessage = "Email is required")]
        [EmailAddress]
        [Display(Name = "Email Address")]
        public string Email { get; set; } = string.Empty;

        [Display(Name = "Address")]
        public string? Address { get; set; }

        [Required(ErrorMessage = "Password is required")]
        [DataType(DataType.Password)]
        [Display(Name = "Password")]
        public string Password { get; set; } = string.Empty;

        [Required(ErrorMessage = "Required date is mandatory")]
        [DataType(DataType.Date)]
        [Display(Name = "Required Date")]
        public DateTime RequiredDate { get; set; }

        [Display(Name = "Urgency Level")]
        public string UrgencyLevel { get; set; } = "Medium";

        [Display(Name = "Application User ID")]
        public string? ApplicationUserId { get; set; }

        public ICollection<BloodRequest>? BloodRequests { get; set; }

        public int Id => RecipientId;
        public string BloodType => BloodGroup;
        public DateTime? RequestDate => RequiredDate;
        public string RequestStatus => "Pending";
        public string? Location { get; set; }
    }
}
