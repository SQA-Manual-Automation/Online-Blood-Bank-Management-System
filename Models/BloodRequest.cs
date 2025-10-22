using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Microsoft.AspNetCore.Mvc.ModelBinding.Validation;

namespace BloodBankSystem.Models
{
    public class BloodRequest
    {
        [Key]
        public int Id { get; set; } // Primary key

        [NotMapped]
        public int RequestId => Id; // Alias for Razor view compatibility

        [Required(ErrorMessage = "Recipient ID is required")]
        public int RecipientId { get; set; }

        [ForeignKey("RecipientId")]
        [ValidateNever]
        public Recipient Recipient { get; set; } = null!;

        [Required(ErrorMessage = "Blood group is required")]
        [StringLength(10, ErrorMessage = "Blood group must be under 10 characters")]
        [Display(Name = "Blood Group")]
        public string BloodGroup { get; set; } = string.Empty;

        [Required(ErrorMessage = "Units required is mandatory")]
        [Range(1, 10, ErrorMessage = "Units must be between 1 and 10")]
        [Display(Name = "Units Required")]
        public int Quantity { get; set; }

        [Required(ErrorMessage = "Location is required")]
        [StringLength(100, ErrorMessage = "Location must be under 100 characters")]
        [Display(Name = "Location")]
        public string Location { get; set; } = string.Empty;

        [StringLength(500, ErrorMessage = "Message must be under 500 characters")]
        [Display(Name = "Message")]
        public string? Message { get; set; }

        [Required(ErrorMessage = "Request date is required")]
        [Display(Name = "Request Date")]
        public DateTime RequestDate { get; set; }

        [Display(Name = "Requested At")]
        public DateTime RequestedAt { get; set; } = DateTime.UtcNow;

        [Display(Name = "Status")]
        public string Status { get; set; } = "Pending";
    }
}
