using System.Threading.Tasks;
using System; // Added for IDisposable
using OnlineBookRental.Domain.Entities; // Needed for IRepository<Book>

namespace OnlineBookRental.Domain.Interfaces
{
    // Interface for the Unit of Work pattern.
    // It encapsulates transactions and saves all changes as a single unit.
    public interface IUnitOfWork : IDisposable // Added IDisposable
    {
        // Exposes the Book repository instance as a generic IRepository<Book>.
        // This is now the sole interface for book data access via the Unit of Work.
        IRepository<Book> Books { get; } // Changed from IBookRepository to IRepository<Book>
        // Saves all pending changes in the unit of work to the database.
        Task<int> CompleteAsync();
    }
}