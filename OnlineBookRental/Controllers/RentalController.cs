using Microsoft.AspNetCore.Mvc;
using OnlineBookRental.Application.Services;
using OnlineBookRental.Domain.Entities;
using OnlineBookRental.Application.Models; // Corrected: Using Application.Models
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using System.Linq; // For FirstOrDefault()
using System.Security.Claims; // For FindFirstValue
using System.Collections.Generic; // For IEnumerable

namespace OnlineBookRental.Web.Controllers
{
    [Authorize] // All actions in this controller require authentication
    public class RentalController : Controller
    {
        private readonly IRentalService _rentalService;

        public RentalController(IRentalService rentalService)
        {
            _rentalService = rentalService;
        }

        // GET: /Rental/ConfirmRental/{rentalHeaderId}
        // Displays the rental details for user confirmation.
        [HttpGet("Rental/ConfirmRental/{rentalHeaderId:int}")] // Explicit route for this action
        public async Task<IActionResult> ConfirmRental(int rentalHeaderId)
        {
            // Fetch the rental header with its details and associated book.
            var rentalHeader = await _rentalService.GetRentalHeaderWithDetails(rentalHeaderId);

            // Check if rentalHeader exists and belongs to the current user.
            if (rentalHeader == null || rentalHeader.ApplicationUserId != User.FindFirstValue(ClaimTypes.NameIdentifier))
            {
                TempData["error"] = "Rental not found or you don't have permission to view it.";
                return RedirectToAction("Index", "Home");
            }

            // Assuming a single book per rental header for simplicity in this initial confirmation.
            // If a cart system is implemented later, this would iterate through RentalDetails.
            var rentalDetail = rentalHeader.RentalDetails.FirstOrDefault();

            if (rentalDetail == null)
            {
                TempData["error"] = "Rental details not found for this rental.";
                return RedirectToAction("Index", "Home");
            }

            // The Book object should now be eagerly loaded via RentalDetail.Book
            var book = rentalDetail.Book;

            var viewModel = new RentalConfirmationViewModel
            {
                RentalHeader = rentalHeader,
                Book = book,
                RentalDurationDays = rentalDetail.RentalDurationDays
            };

            // Explicitly specify the view path to avoid view lookup issues
            return View("~/Views/Rental/ConfirmRental.cshtml", viewModel);
        }

        // POST: /Rental/FinalizeRental
        // Finalizes the rental transaction.
        [HttpPost("Rental/FinalizeRental")] // Explicit route for this action
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> FinalizeRental(int rentalHeaderId)
        {
            var success = await _rentalService.ConfirmRental(rentalHeaderId); // This will eventually update book quantity etc.

            if (success)
            {
                TempData["success"] = "Book rented successfully! Enjoy your read.";
                // Redirect to the "My Rentals" page after successful finalization
                return RedirectToAction(nameof(MyRentals));
            }
            else
            {
                TempData["error"] = "Failed to finalize rental. Please try again.";
                return RedirectToAction(nameof(ConfirmRental), new { rentalHeaderId = rentalHeaderId });
            }
        }

        // GET: /Rental/MyRentals
        // Displays a list of all rentals for the currently logged-in user.
        [HttpGet] // Explicitly define this as an HTTP GET action
        public async Task<IActionResult> MyRentals()
        {
            var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
            if (string.IsNullOrEmpty(userId))
            {
                TempData["error"] = "You must be logged in to view your rentals.";
                return RedirectToPage("/Account/Login", new { area = "Identity" });
            }

            // Retrieve the rentals for the current user
            IEnumerable<RentalViewModel> userRentals = await _rentalService.GetUserRentalsAsync(userId);

            return View(userRentals);
        }
    }
}