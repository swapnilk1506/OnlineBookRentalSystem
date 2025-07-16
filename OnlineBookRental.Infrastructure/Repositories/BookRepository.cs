using OnlineBookRental.Domain.Entities;
// Note: IBookRepository is NOT used here directly if it's removed.
// This class will now implicitly implement IRepository<Book> through GenericRepository<Book>.
using OnlineBookRental.Infrastructure.Data;

namespace OnlineBookRental.Infrastructure.Repositories
{
    // Specific repository implementation for Book entities.
    // It inherits from GenericRepository for common operations.
    // IMPORTANT: If IBookRepository is removed, this class should NOT implement it directly.
    // It will still be used as the concrete implementation for IRepository<Book> or directly.
    public class BookRepository : GenericRepository<Book> // Removed IBookRepository from inheritance
    {
        // Constructor to pass the database context to the base generic repository.
        public BookRepository(ApplicationDbContext context) : base(context)
        {
        }

        // Add book-specific data access methods here if needed.
        // For example:
        // public Task<IEnumerable<Book>> GetBooksByAuthorAsync(string author)
        // {
        //     return _dbSet.Where(b => b.Author == author).ToListAsync();
        // }
    }
}