using Microsoft.AspNetCore.Mvc;
using OnlineBookRental.Application.Services; // Required for IBookService
using OnlineBookRental.Domain.Entities;     // Required for Book entity
using System.Diagnostics;
using OnlineBookRental.Web.Models;          // Required for ErrorViewModel
using System.Collections.Generic;           // Required for IEnumerable
using System.Linq;                          // Required for LINQ operations like Where
using System.Threading.Tasks;               // Required for async/await

namespace OnlineBookRental.Web.Controllers
{
    public class HomeController : Controller
    {
        private readonly ILogger<HomeController> _logger;
        private readonly IBookService _bookService;

        // Constructor: Injects ILogger and IBookService
        public HomeController(ILogger<HomeController> logger, IBookService bookService)
        {
            _logger = logger;
            _bookService = bookService;
        }

        // Index Action: Fetches and displays books, now with optional search filtering.
        // The 'searchString' parameter captures the user's input from the search box.
        public async Task<IActionResult> Index(string? searchString) // Added nullable string parameter
        {
            // Fetch all books from the database using the BookService.
            IEnumerable<Book> books = await _bookService.GetAllBooksAsync();

            // If a search string is provided and is not empty, filter the books.
            if (!string.IsNullOrEmpty(searchString))
            {
                // Convert the search string to lowercase for case-insensitive comparison.
                string lowerSearchString = searchString.ToLower();

                // Filter the 'books' collection based on Title, Author, or ISBN.
                // The .Contains() method checks if the string contains the search term.
                // .ToList() is used to execute the query and get the results immediately.
                books = books.Where(b =>
                    b.Title.ToLower().Contains(lowerSearchString) ||
                    b.Author.ToLower().Contains(lowerSearchString) ||
                    b.ISBN.ToLower().Contains(lowerSearchString)
                ).ToList();
            }

            // Store the current search string in ViewData. This allows the search box
            // on the view to retain the last entered value after a search is performed.
            ViewData["CurrentFilter"] = searchString;

            // Pass the (potentially filtered) list of books to the Index view.
            return View(books);
        }

        // Privacy Action: Displays the privacy policy page.
        public IActionResult Privacy()
        {
            return View();
        }

        // Error Action: Displays error pages, with caching disabled for immediate updates.
        [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
        public IActionResult Error()
        {
            // Creates an ErrorViewModel with the current request ID for debugging.
            return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
        }
    }
}