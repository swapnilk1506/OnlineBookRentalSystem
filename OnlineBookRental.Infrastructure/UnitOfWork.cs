using OnlineBookRental.Domain.Interfaces;
using OnlineBookRental.Infrastructure.Data;
using OnlineBookRental.Infrastructure.Repositories;
using System.Threading.Tasks;
using System; // Added for IDisposable
using OnlineBookRental.Domain.Entities; // Added for IRepository<Book>

namespace OnlineBookRental.Infrastructure
{
    // Implementation of the Unit of Work pattern.
    // Manages repository instances and database transaction commits.
    public class UnitOfWork : IUnitOfWork // Now explicitly implements IDisposable
    {
        // The database context instance.
        private readonly ApplicationDbContext _context;
        // Private field for the Book repository instance, now generic.
        private IRepository<Book>? _bookRepository; // Changed to IRepository<Book>

        // Constructor to initialize the database context.
        public UnitOfWork(ApplicationDbContext context)
        {
            _context = context;
        }

        // Public property to get the Book repository instance.
        // Lazy initialization ensures the repository is created only when first accessed.
        public IRepository<Book> Books // Changed return type to IRepository<Book>
        {
            get
            {
                if (_bookRepository == null)
                {
                    // Directly instantiate BookRepository which implements IRepository<Book>
                    _bookRepository = new BookRepository(_context);
                }
                return _bookRepository;
            }
        }

        // Saves all changes made in this unit of work to the database.
        // Returns the number of state entries written to the database.
        public async Task<int> CompleteAsync()
        {
            return await _context.SaveChangesAsync();
        }

        // Disposes the database context when the unit of work is no longer needed.
        public void Dispose()
        {
            _context.Dispose();
        }
    }
}