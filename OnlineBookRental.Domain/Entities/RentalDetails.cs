using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace OnlineBookRental.Domain.Entities
{
    // Represents the details of a specific book within a rental transaction.
    public class RentalDetail
    {
        [Key] // Primary key for the RentalDetail table
        public int Id { get; set; }

        // Foreign key to the RentalHeader this detail belongs to.
        [Required]
        public int RentalHeaderId { get; set; }
        [ForeignKey("RentalHeaderId")] // Specify the foreign key relationship
        public RentalHeader RentalHeader { get; set; } = null!; // Navigation property to the rental header

        // Foreign key to the Book being rented.
        [Required]
        public int BookId { get; set; }
        [ForeignKey("BookId")] // Specify the foreign key relationship
        public Book Book { get; set; } = null!; // Navigation property to the book

        [Required]
        [Range(0.01, double.MaxValue, ErrorMessage = "Price per day must be greater than 0.")]
        public decimal PricePerDay { get; set; } // The rental price per day for this specific book (at the time of rental)

        [Required]
        [Range(1, int.MaxValue, ErrorMessage = "Rental duration must be at least 1 day.")]
        public int RentalDurationDays { get; set; } // How many days the book is rented for

        [Required]
        [Range(0.01, double.MaxValue, ErrorMessage = "Total detail amount must be greater than 0.")]
        public decimal TotalDetailAmount { get; set; } // Total cost for this specific book detail (PricePerDay * RentalDurationDays)
    }
}