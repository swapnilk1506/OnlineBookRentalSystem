using OnlineBookRental.Domain.Entities;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace OnlineBookRental.Application.Services
{
    // Interface for the Book Service, defining operations related to books.
    public interface IBookService
    {
        // Retrieves all books.
        Task<IEnumerable<Book>> GetAllBooksAsync();
        // Retrieves a book by its ID.
        Task<Book?> GetBookByIdAsync(int id);
        // Adds a new book.
        Task AddBookAsync(Book book);
        // Updates an existing book.
        Task UpdateBookAsync(Book book);
        // Deletes a book by its ID.
        Task DeleteBookAsync(int id);
    }
}