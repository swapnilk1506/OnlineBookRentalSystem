using OnlineBookRental.Domain.Entities;
using OnlineBookRental.Domain.Interfaces;
using OnlineBookRental.Infrastructure.Data;

namespace OnlineBookRental.Infrastructure.Repositories
{
    // Specific repository for RentalDetail entities.
    public class RentalDetailRepository : GenericRepository<RentalDetail>, IRepository<RentalDetail>
    {
        public RentalDetailRepository(ApplicationDbContext context) : base(context)
        {
        }
    }
}