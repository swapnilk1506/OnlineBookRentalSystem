using Microsoft.AspNetCore.Mvc;
using OnlineBookRental.Application.Services;
using OnlineBookRental.Domain.Entities;
using System.Threading.Tasks;
using System.Collections.Generic;//required for IEnumerable

namespace OnlineBookRental.web.Controllers
{
    public class BookController : Controller
    {
        //service to interact with book related buisnesslogic
        private readonly IBookService _bookService;

        //Constructor to inject the IBookService dependency
        public BookController(IBookService bookService)
        {
            _bookService = bookService;
        }
        //Get:/Book/Index
        //Display a list of books.This will serve as the book management dashboard
        public async Task<IActionResult> Index()
        {
            IEnumerable<Book> books = await _bookService.GetAllBooksAsync();
            return View(books);
        }

        //Get:/Book/Create
        //Displays a form to create a new book
        public IActionResult Create()
        {
            //Simply return the create view.It will contain the form fields
            return View();
        }

        //Post:/Book/Create
        //Handles the submission of the new book form
        [HttpPost]
        [ValidateAntiForgeryToken]//protects againts the cross-site request forgery attacks
        public async Task<IActionResult> Create(Book book)
        {
            //check if the submitted book data is valid
            if (ModelState.IsValid)
            {
                //Add new book using the BookService
                await _bookService.AddBookAsync(book);
                //Redirect to the Index Action to show the updated list of books
                TempData["Success"] = "Book Added Successfully!";
                return RedirectToAction(nameof(Index));
            }
            return View(book);
        }

        //Get:/Book/Edit/{id}
        //Display the form to edit an existing book
        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null || id == 0)
            {
                return NotFound();
            }

            Book? bookFromDb = await _bookService.GetBookByIdAsync(id.Value);

            if (bookFromDb == null)
            {
                return NotFound();
            }
            return View(bookFromDb);
        }

        //Post: /Book/Edit
        //Handles the submission of the edited book form
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(Book book)
        {
            //check if the submitted book data is valid
            if (ModelState.IsValid)
            {
                //update the book using BookServices
                await _bookService.UpdateBookAsync(book);

                //Redirect to the Index action to show the updated list of books
                TempData["success"] = "Book Updated Successfully!";
                return RedirectToAction(nameof(Index));
            }
            //If the model is not valid, return the same view with the book data
            return View(book);
        }

        //Get: /Book/Delete/{id}
        //Display the confirmation page to delete a book
        public async Task<IActionResult> Delete(int? id)
        {
            //if no id is provided or ID is 0, return NotFound
            if (id == null || id == 0)
            {
                return NotFound();
            }

            //Retrive the book from the database using the BookService
            Book? bookFromDb = await _bookService.GetBookByIdAsync(id.Value);

            //if book is not found, return NotFound
            if (bookFromDb == null)
            {
                return NotFound();
            }

            //Pass the retrived book to the Delete view for confirmation
            return View(bookFromDb);
        }

        //POST: /Book/DeleteConfirmed
        [HttpPost, ActionName("Delete")]//Mapping this post action to the Delete view
        [ValidateAntiForgeryToken]//protects againts cross-site request forgery attacks
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            Book? bookFromDb = await _bookService.GetBookByIdAsync(id);

            //if book is notfound , return not found
            if (bookFromDb == null)
            {
                return NotFound();
            }

            //Delete the book using the BookService
            await _bookService.DeleteBookAsync(id);
            TempData["Success"] = "Book Deleted Successfully";
            return RedirectToAction(nameof(Index));
        }


    }
}
