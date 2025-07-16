using OnlineBookRental.Domain.Entities;
using System.ComponentModel.DataAnnotations;

namespace OnlineBookRental.Application.Models // Corrected: Namespace is now in Application layer
{
    // ViewModel to display rental details for confirmation before final checkout.
    public class RentalConfirmationViewModel
    {
        public RentalHeader RentalHeader { get; set; } = null!;
        public Book Book { get; set; } = null!; // Assuming for now it's a single book rental for confirmation

        [Display(Name = "Rental Duration (Days)")]
        [Range(1, 30, ErrorMessage = "Rental duration must be between 1 and 30 days.")]
        public int RentalDurationDays { get; set; } = 7; // Default to 7 days
    }
}