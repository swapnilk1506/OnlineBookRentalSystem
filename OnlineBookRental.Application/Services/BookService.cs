using OnlineBookRental.Application.Services;
using OnlineBookRental.Domain.Entities;
using OnlineBookRental.Domain.Interfaces;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace OnlineBookRental.Application.Services
{
    // Implementation of the Book Service.
    // It uses the Unit of Work to interact with the data layer (repositories).
    public class BookService : IBookService
    {
        // The Unit of Work instance, injected via dependency injection.
        private readonly IUnitOfWork _unitOfWork;

        // Constructor to inject the Unit of Work.
        public BookService(IUnitOfWork unitOfWork)
        {
            _unitOfWork = unitOfWork;
        }

        // Retrieves all books from the repository.
        // This method was previously throwing NotImplementedException.
        public async Task<IEnumerable<Book>> GetAllBooksAsync()
        {
            // Now correctly fetches all books using the Book repository from the Unit of Work.
            return await _unitOfWork.Books.GetAllAsync();
        }

        // Retrieves a book by ID from the repository.
        public async Task<Book?> GetBookByIdAsync(int id)
        {
            // Now correctly fetches a book by ID using the Book repository from the Unit of Work.
            return await _unitOfWork.Books.GetByIdAsync(id);
        }

        // Adds a new book and persists changes.
        public async Task AddBookAsync(Book book)
        {
            // Now correctly adds a book using the Book repository from the Unit of Work.
            await _unitOfWork.Books.AddAsync(book);
            await _unitOfWork.CompleteAsync();
        }

        // Updates an existing book and persists changes.
        public async Task UpdateBookAsync(Book book)
        {
            // Now correctly updates a book using the Book repository from the Unit of Work.
            await _unitOfWork.Books.UpdateAsync(book);
            await _unitOfWork.CompleteAsync();
        }

        // Deletes a book by ID and persists changes.
        public async Task DeleteBookAsync(int id)
        {
            // Now correctly deletes a book by ID using the Book repository from the Unit of Work.
            await _unitOfWork.Books.DeleteAsync(id);
            await _unitOfWork.CompleteAsync();
        }
    }
}