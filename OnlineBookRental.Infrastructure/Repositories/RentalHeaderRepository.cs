using OnlineBookRental.Domain.Entities;
using OnlineBookRental.Domain.Interfaces;
using OnlineBookRental.Infrastructure.Data;

namespace OnlineBookRental.Infrastructure.Repositories
{
    // Specific repository for RentalHeader entities.
    public class RentalHeaderRepository : GenericRepository<RentalHeader>, IRepository<RentalHeader>
    {
        public RentalHeaderRepository(ApplicationDbContext context) : base(context)
        {
        }
    }
}