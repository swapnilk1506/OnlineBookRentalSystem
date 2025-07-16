using Microsoft.AspNetCore.Mvc;
using OnlineBookRental.Application.Services;
using OnlineBookRental.Domain.Entities;
using OnlineBookRental.Application.Models; // Corrected: Using Application.Models for RentalConfirmationViewModel
using System.Threading.Tasks;
using System.Collections.Generic; // Required for IEnumerable
using Microsoft.AspNetCore.Authorization; // Required for [Authorize]

namespace OnlineBookRental.Web.Controllers
{
    // Controller for managing book-related operations.
    // In a real application, this would typically be protected by authorization (e.g., [Authorize(Roles = "Admin")]).
    public class BookController : Controller
    {
        // Service to interact with book-related business logic.
        private readonly IBookService _bookService;
        // NEW: Service to interact with rental-related business logic.
        private readonly IRentalService _rentalService;

        // Constructor to inject the IBookService and IRentalService.
        public BookController(IBookService bookService, IRentalService rentalService)
        {
            _bookService = bookService;
            _rentalService = rentalService;
        }

        // GET: /Book/Index
        // Displays a list of all books. This will serve as the book management dashboard.
        public async Task<IActionResult> Index()
        {
            // Retrieve all books using the BookService.
            IEnumerable<Book> books = await _bookService.GetAllBooksAsync();
            // Pass the list of books to the view.
            return View(books);
        }

        // GET: /Book/Details/{id}
        // Displays the details of a single book. This action is accessible to all users.
        public async Task<IActionResult> Details(int? id)
        {
            // If no ID is provided or ID is 0, return NotFound.
            if (id == null || id == 0)
            {
                return NotFound();
            }

            // Retrieve the book from the database using its ID.
            Book? bookFromDb = await _bookService.GetBookByIdAsync(id.Value); // Corrected method call

            // If the book is not found, return NotFound.
            if (bookFromDb == null)
            {
                return NotFound();
            }

            // Pass the retrieved book to the Details view.
            return View(bookFromDb);
        }

        // POST: /Book/InitiateRental
        // Handles the "Rent Now!" button click from the Book Details page.
        // Requires authentication.
        [HttpPost]
        [Authorize] // Only authenticated users can initiate a rental
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> InitiateRental(int bookId, int rentalDurationDays = 7)
        {
            // Initiate the rental via the RentalService.
            // Pass the bookId, current user, and desired rental duration.
            var (rentalHeader, errorMessage) = await _rentalService.InitiateRental(bookId, User, rentalDurationDays);

            if (rentalHeader == null)
            {
                // If there's an error message, display it. Otherwise, use a generic one.
                TempData["error"] = errorMessage ?? "Could not initiate rental. An unknown error occurred.";
                return RedirectToAction(nameof(Details), new { id = bookId });
            }

            // Redirect to a confirmation page, passing the RentalHeaderId.
            return RedirectToAction("ConfirmRental", "Rental", new { rentalHeaderId = rentalHeader.Id });
        }


        // GET: /Book/Create
        // Displays the form to create a new book.
        public IActionResult Create()
        {
            // Simply return the Create view. It will contain the form fields.
            return View();
        }

        // POST: /Book/Create
        // Handles the submission of the new book form.
        [HttpPost]
        [ValidateAntiForgeryToken] // Protects against Cross-Site Request Forgery attacks.
        public async Task<IActionResult> Create(Book book)
        {
            // Check if the submitted book data is valid according to its model annotations.
            if (ModelState.IsValid)
            {
                // Add the new book using the BookService.
                await _bookService.AddBookAsync(book);
                // Redirect to the Index action to show the updated list of books.
                TempData["success"] = "Book created successfully!"; // Optional: Add a success message
                return RedirectToAction(nameof(Index));
            }
            // If the model is not valid, return the same view with the entered data and validation errors.
            return View(book);
        }

        // GET: /Book/Edit/{id}
        // Displays the form to edit an existing book.
        public async Task<IActionResult> Edit(int? id)
        {
            // If no ID is provided or ID is 0, return NotFound.
            if (id == null || id == 0)
            {
                return NotFound();
            }

            // Retrieve the book from the database using its ID.
            Book? bookFromDb = await _bookService.GetBookByIdAsync(id.Value); // Corrected method call

            // If the book is not found, return NotFound.
            if (bookFromDb == null)
            {
                return NotFound();
            }

            // Pass the retrieved book to the Edit view.
            return View(bookFromDb);
        }

        // POST: /Book/Edit
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(Book book)
        {
            // Check if the submitted book data is valid.
            if (ModelState.IsValid)
            {
                // Update the book using the BookService.
                await _bookService.UpdateBookAsync(book);
                // Redirect to the Index action to show the updated list.
                TempData["success"] = "Book updated successfully!";
                return RedirectToAction(nameof(Index));
            }
            // If the model is not valid, return the same view with the entered data and validation errors.
            return View(book);
        }

        // GET: /Book/Delete/{id}
        // Displays the confirmation page for deleting a book.
        public async Task<IActionResult> Delete(int? id)
        {
            // If no ID is provided or ID is 0, return NotFound.
            if (id == null || id == 0)
            {
                return NotFound();
            }

            // Retrieve the book from the database.
            Book? bookFromDb = await _bookService.GetBookByIdAsync(id.Value); // Corrected method call

            // If the book is not found (e.g., already deleted by another user), return NotFound.
            if (bookFromDb == null)
            {
                return NotFound();
            }

            // Pass the retrieved book to the Delete view for confirmation.
            return View(bookFromDb);
        }

        // POST: /Book/DeleteConfirmed
        // Handles the actual deletion of the book after confirmation.
        [HttpPost, ActionName("Delete")] // Map this POST action to the "Delete" GET action name.
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            // Retrieve the book to be deleted.
            Book? bookFromDb = await _bookService.GetBookByIdAsync(id); // Corrected method call

            // If the book is not found (e.g., already deleted by another user), return NotFound.
            if (bookFromDb == null)
            {
                return NotFound();
            }

            // Delete the book using the BookService.
            await _bookService.DeleteBookAsync(id);
            // Redirect to the Index action.
            TempData["success"] = "Book deleted successfully!";
            return RedirectToAction(nameof(Index));
        }
    }
}