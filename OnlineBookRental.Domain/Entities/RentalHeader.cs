using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Microsoft.AspNetCore.Identity; // For ApplicationUser

namespace OnlineBookRental.Domain.Entities
{
    // Represents the header information for a rental transaction.
    public class RentalHeader
    {
        [Key] // Primary key for the RentalHeader table
        public int Id { get; set; }

        // Foreign key to the ApplicationUser who made the rental.
        public string ApplicationUserId { get; set; } = string.Empty;
        [ForeignKey("ApplicationUserId")] // Specify the foreign key relationship
        public ApplicationUser ApplicationUser { get; set; } = null!; // Navigation property to the user

        [Required]
        public DateTime RentalDate { get; set; } = DateTime.Now; // Date when the rental was initiated

        [Required]
        public DateTime DueDate { get; set; } // Date when the book is due back

        public DateTime? ReturnDate { get; set; } // Nullable: Date when the book was actually returned

        [Required]
        [Range(0.01, double.MaxValue, ErrorMessage = "Total amount must be greater than 0.")]
        public decimal TotalRentalAmount { get; set; } // Total cost of the rental transaction

        // Status of the rental (e.g., Pending, Active, Returned, Overdue, Cancelled)
        public string RentalStatus { get; set; } = "Pending"; // Initial status

        // Navigation property to hold the collection of RentalDetails for this header.
        // This is crucial for eager loading related details.
        public ICollection<RentalDetail> RentalDetails { get; set; } = new List<RentalDetail>();
    }
}