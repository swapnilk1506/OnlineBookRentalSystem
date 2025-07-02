using OnlineBookRental.Domain.Entities;
using OnlineBookRental.Domain.Interfaces; // Still needed for IRepository<Book>
using OnlineBookRental.Infrastructure.Data;

namespace OnlineBookRental.Infrastructure.Repositories
{
    // Specific repository implementation for Book entities.
    // It now only inherits from GenericRepository<Book> and directly provides IRepository<Book> functionality.
    // The separate IBookRepository interface is removed to simplify DI.
    public class BookRepository : GenericRepository<Book> // Removed IBookRepository from inheritance
    {
        // Constructor to pass the database context to the base generic repository.
        public BookRepository(ApplicationDbContext context) : base(context)
        {
        }

        // Any book-specific data access methods would go here, if they existed.
        // They would be called via IRepository<Book> from the Unit of Work.
    }
}