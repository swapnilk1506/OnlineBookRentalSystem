using Microsoft.AspNetCore.Mvc;
using OnlineBookRental.Application.Services;
using OnlineBookRental.Domain.Entities;
using OnlineBookRental.Web.Models;
using System.Diagnostics;

namespace OnlineBookRental.Web.Controllers
{
    // Main controller for the home page.
    public class HomeController : Controller
    {
        // Service to interact with book-related operations.
        private readonly IBookService _bookService;

        // Constructor to inject the Book Service.
        public HomeController(IBookService bookService)
        {
            _bookService = bookService;
        }

        // Action method for the home page.
        // Retrieves all books and passes them to the view.
        public async Task<IActionResult> Index()
        {
            // Fetch all books using the Book Service.
            IEnumerable<Book> books = await _bookService.GetAllBooksAsync();
            return View(books); // Pass the list of books to the view.
        }

        // Action method for the privacy policy page.
        public IActionResult Privacy()
        {
            return View();
        }

        // Action method for displaying error pages.
        [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
        public IActionResult Error()
        {
            return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
        }
    }
}