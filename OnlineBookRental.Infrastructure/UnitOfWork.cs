using OnlineBookRental.Domain.Entities;
using OnlineBookRental.Domain.Interfaces;
using OnlineBookRental.Infrastructure.Data;
using OnlineBookRental.Infrastructure.Repositories;
using System.Threading.Tasks;

namespace OnlineBookRental.Infrastructure
{
    // Implementation of the Unit of Work pattern.
    // Manages repository instances and database transaction commits.
    public class UnitOfWork : IUnitOfWork
    {
        // The database context instance.
        private readonly ApplicationDbContext _context;
        // Private fields for repository instances.
        private IRepository<Book>? _bookRepository;
        private IRepository<RentalHeader>? _rentalHeaderRepository;
        private IRepository<RentalDetail>? _rentalDetailRepository;

        // Constructor to initialize the database context.
        public UnitOfWork(ApplicationDbContext context)
        {
            _context = context;
        }

        // Public property to get the Book repository instance.
        public IRepository<Book> Books
        {
            get
            {
                if (_bookRepository == null)
                {
                    _bookRepository = new BookRepository(_context);
                }
                return _bookRepository;
            }
        }

        // Public property to get the RentalHeader repository instance.
        public IRepository<RentalHeader> RentalHeaders
        {
            get
            {
                if (_rentalHeaderRepository == null)
                {
                    _rentalHeaderRepository = new RentalHeaderRepository(_context);
                }
                return _rentalHeaderRepository;
            }
        }

        // Public property to get the RentalDetail repository instance.
        public IRepository<RentalDetail> RentalDetails
        {
            get
            {
                if (_rentalDetailRepository == null)
                {
                    _rentalDetailRepository = new RentalDetailRepository(_context);
                }
                return _rentalDetailRepository;
            }
        }

        // Saves all changes made in this unit of work to the database.
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