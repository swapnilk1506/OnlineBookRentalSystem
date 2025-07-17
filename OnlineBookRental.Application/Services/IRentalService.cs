using OnlineBookRental.Domain.Entities;
using OnlineBookRental.Application.Models; // Corrected: Using Application.Models
using System.Collections.Generic; // Required for IEnumerable
using System.Threading.Tasks;
using System.Security.Claims; // For ClaimsPrincipal

namespace OnlineBookRental.Application.Services
{
    // Interface for the Rental Service, defining operations related to book rentals.
    public interface IRentalService
    {
        // Initiates a new rental, typically creating a RentalHeader and RentalDetail.
        // Returns the created RentalHeader or a ViewModel for confirmation.
        // Returns a tuple: (RentalHeader if successful, ErrorMessage if failed)
        Task<(RentalHeader? RentalHeader, string? ErrorMessage)> InitiateRental(int bookId, ClaimsPrincipal user, int rentalDurationDays = 7);
        // Confirms a rental, finalizing the transaction and updating book availability.
        Task<bool> ConfirmRental(int rentalHeaderId);
        // Retrieves a rental header with its details.
        Task<RentalHeader?> GetRentalHeaderWithDetails(int rentalHeaderId);
        // Retrieves all rental headers (with details and books) for a specific user.
        Task<IEnumerable<RentalViewModel>> GetUserRentalsAsync(string userId);
        // NEW: Handles the return of a book, updating rental status and book quantity.
        Task<(bool Success, string? ErrorMessage)> ReturnBook(int rentalHeaderId);
    }
}