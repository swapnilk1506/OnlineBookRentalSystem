using Microsoft.EntityFrameworkCore; // Required for Include()
using OnlineBookRental.Domain.Entities;
using OnlineBookRental.Domain.Interfaces;
using OnlineBookRental.Infrastructure.Data; // Required for ApplicationDbContext
using OnlineBookRental.Application.Models; // Corrected: Using Application.Models
using System.Threading.Tasks;
using System.Security.Claims; // For ClaimsPrincipal
using System; // For DateTime
using System.Linq; // For FirstOrDefaultAsync, Select
using System.Collections.Generic; // For List

namespace OnlineBookRental.Application.Services
{
    // Implementation of the Rental Service.
    // It orchestrates rental operations using the Unit of Work.
    public class RentalService : IRentalService
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly ApplicationDbContext _context; // Inject DbContext directly for eager loading

        public RentalService(IUnitOfWork unitOfWork, ApplicationDbContext context) // Inject ApplicationDbContext
        {
            _unitOfWork = unitOfWork;
            _context = context; // Assign injected DbContext
        }

        // Initiates a new rental for a single book.
        // This method will create a RentalHeader and a RentalDetail.
        public async Task<(RentalHeader? RentalHeader, string? ErrorMessage)> InitiateRental(int bookId, ClaimsPrincipal user, int rentalDurationDays = 7)
        {
            // 1. Ensure the user is logged in
            if (user == null || !user.Identity!.IsAuthenticated)
            {
                return (null, "User not authenticated. Please log in to rent a book.");
            }

            // Get the ApplicationUserId from the ClaimsPrincipal
            var userId = user.FindFirstValue(ClaimTypes.NameIdentifier);
            if (string.IsNullOrEmpty(userId))
            {
                return (null, "User ID not found. Please log in again.");
            }

            // 2. Check if the user already has an ACTIVE rental for this specific book
            var existingActiveRental = await _context.RentalHeaders
                .Where(rh => rh.ApplicationUserId == userId && rh.RentalStatus == "Active")
                .Include(rh => rh.RentalDetails)
                .AnyAsync(rh => rh.RentalDetails.Any(rd => rd.BookId == bookId));

            if (existingActiveRental)
            {
                return (null, "You have already rented this book. Please return it before renting again.");
            }

            // 3. Retrieve the book details
            var book = await _unitOfWork.Books.GetByIdAsync(bookId);
            if (book == null)
            {
                return (null, "Book not found.");
            }
            if (book.QuantityAvailable <= 0)
            {
                return (null, "The requested book is currently out of stock.");
            }

            // 4. Calculate due date
            DateTime rentalDate = DateTime.Now;
            DateTime dueDate = rentalDate.AddDays(rentalDurationDays);
            decimal totalDetailAmount = book.RentalPricePerDay * rentalDurationDays;

            // 5. Create RentalHeader
            var rentalHeader = new RentalHeader
            {
                ApplicationUserId = userId,
                RentalDate = rentalDate,
                DueDate = dueDate,
                TotalRentalAmount = totalDetailAmount, // For single item, total is just this item's total
                RentalStatus = "Pending" // Initial status
            };

            await _unitOfWork.RentalHeaders.AddAsync(rentalHeader);
            await _unitOfWork.CompleteAsync(); // Save header to get its Id

            // 6. Create RentalDetail
            var rentalDetail = new RentalDetail
            {
                RentalHeaderId = rentalHeader.Id,
                BookId = book.Id,
                PricePerDay = book.RentalPricePerDay,
                RentalDurationDays = rentalDurationDays,
                TotalDetailAmount = totalDetailAmount
            };

            await _unitOfWork.RentalDetails.AddAsync(rentalDetail);
            await _unitOfWork.CompleteAsync(); // Save detail

            // 7. Decrement book quantity (CRITICAL for actual rental)
            book.QuantityAvailable--;
            await _unitOfWork.Books.UpdateAsync(book);
            await _unitOfWork.CompleteAsync();

            return (rentalHeader, null); // Return the created rental header and no error message
        }

        // Confirms a rental, finalizing the transaction and updating book availability.
        public async Task<bool> ConfirmRental(int rentalHeaderId)
        {
            var rentalHeader = await _unitOfWork.RentalHeaders.GetByIdAsync(rentalHeaderId);
            if (rentalHeader == null)
            {
                return false; // Rental header not found
            }

            // Update the status to Active
            rentalHeader.RentalStatus = "Active";
            await _unitOfWork.RentalHeaders.UpdateAsync(rentalHeader);
            await _unitOfWork.CompleteAsync(); // Persist the status change

            return true;
        }

        // Retrieves a rental header with its details, eagerly loading RentalDetails and Book entities.
        public async Task<RentalHeader?> GetRentalHeaderWithDetails(int rentalHeaderId)
        {
            // Use _context directly for eager loading with Include()
            // IMPORTANT: Ensure RentalDetails navigation property is correctly defined in RentalHeader.cs
            return await _context.RentalHeaders
                                 .Include(rh => rh.RentalDetails) // Include the collection of rental details
                                     .ThenInclude(rd => rd.Book) // Then include the Book for each rental detail
                                 .FirstOrDefaultAsync(rh => rh.Id == rentalHeaderId);
        }

        // Retrieves all rental headers (with details and books) for a specific user.
        public async Task<IEnumerable<RentalViewModel>> GetUserRentalsAsync(string userId)
        {
            // Fetch rental headers for the user, including related details and book info
            var rentalHeaders = await _context.RentalHeaders
                                              .Where(rh => rh.ApplicationUserId == userId)
                                              .Include(rh => rh.RentalDetails)
                                                  .ThenInclude(rd => rd.Book)
                                              .OrderByDescending(rh => rh.RentalDate) // Order by most recent rentals
                                              .ToListAsync();

            // Project the entities into a list of RentalViewModel
            var rentalViewModels = rentalHeaders.Select(rh => new RentalViewModel
            {
                RentalHeaderId = rh.Id,
                BookTitle = rh.RentalDetails.FirstOrDefault()?.Book?.Title ?? "N/A", // Assuming one book per header for simplicity
                Author = rh.RentalDetails.FirstOrDefault()?.Book?.Author ?? "N/A",
                RentalDate = rh.RentalDate,
                DueDate = rh.DueDate,
                ReturnDate = rh.ReturnDate,
                TotalAmount = rh.TotalRentalAmount,
                RentalStatus = rh.RentalStatus
            }).ToList();

            return rentalViewModels;
        }

        // NEW: Handles the return of a book, updating rental status and book quantity.
        public async Task<(bool Success, string? ErrorMessage)> ReturnBook(int rentalHeaderId)
        {
            // 1. Retrieve the rental header with its details and associated book
            var rentalHeader = await _context.RentalHeaders
                                             .Include(rh => rh.RentalDetails)
                                                 .ThenInclude(rd => rd.Book)
                                             .FirstOrDefaultAsync(rh => rh.Id == rentalHeaderId);

            if (rentalHeader == null)
            {
                return (false, "Rental not found.");
            }

            if (rentalHeader.RentalStatus == "Returned" || rentalHeader.RentalStatus == "Expired")
            {
                return (false, "This rental has already been returned or expired.");
            }

            // 2. Update RentalHeader
            rentalHeader.ReturnDate = DateTime.Now;
            rentalHeader.RentalStatus = "Returned";
            await _unitOfWork.RentalHeaders.UpdateAsync(rentalHeader);

            // 3. Update Book Quantity
            // Assuming one rental detail per header for simplicity in this context.
            // In a more complex system with multiple books per rental, this would loop through details.
            var rentalDetail = rentalHeader.RentalDetails.FirstOrDefault();
            if (rentalDetail != null && rentalDetail.Book != null)
            {
                rentalDetail.Book.QuantityAvailable++;
                await _unitOfWork.Books.UpdateAsync(rentalDetail.Book);
            }
            else
            {
                return (false, "Could not find associated book details for this rental.");
            }

            // 4. Complete the unit of work to save all changes
            await _unitOfWork.CompleteAsync();

            return (true, null); // Return success
        }
    }
}