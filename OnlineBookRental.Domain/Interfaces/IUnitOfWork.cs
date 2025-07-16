using System.Threading.Tasks;
using OnlineBookRental.Domain.Entities;

namespace OnlineBookRental.Domain.Interfaces
{
    // Interface for the Unit of Work pattern.
    // It encapsulates transactions and saves all changes as a single unit.
    public interface IUnitOfWork
    {
        // Exposes the Book repository instance.
        IRepository<Book> Books { get; }
        // NEW: Exposes the RentalHeader repository instance.
        IRepository<RentalHeader> RentalHeaders { get; }
        // NEW: Exposes the RentalDetail repository instance.
        IRepository<RentalDetail> RentalDetails { get; }

        // Saves all pending changes in the unit of work to the database.
        Task<int> CompleteAsync();
    }
}