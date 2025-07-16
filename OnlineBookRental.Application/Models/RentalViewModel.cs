using System;
using System.ComponentModel.DataAnnotations;

namespace OnlineBookRental.Application.Models // Corrected: Namespace is now in Application layer
{
    // ViewModel to represent a single rental entry for display on "My Rentals" page.
    public class RentalViewModel
    {
        public int RentalHeaderId { get; set; }

        [Display(Name = "Book Title")]
        public string BookTitle { get; set; } = string.Empty;

        public string Author { get; set; } = string.Empty;

        [Display(Name = "Rental Date")]
        [DataType(DataType.Date)]
        public DateTime RentalDate { get; set; }

        [Display(Name = "Due Date")]
        [DataType(DataType.Date)]
        public DateTime DueDate { get; set; }

        [Display(Name = "Return Date")]
        [DataType(DataType.Date)]
        public DateTime? ReturnDate { get; set; } // Nullable

        [Display(Name = "Total Amount")]
        [DataType(DataType.Currency)]
        public decimal TotalAmount { get; set; }

        [Display(Name = "Status")]
        public string RentalStatus { get; set; } = string.Empty;

        // Helper property to determine if the book is overdue
        public bool IsOverdue => RentalStatus == "Active" && DateTime.Now > DueDate;
    }
}