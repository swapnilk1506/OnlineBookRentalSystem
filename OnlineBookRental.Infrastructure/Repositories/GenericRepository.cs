using Microsoft.EntityFrameworkCore;
using OnlineBookRental.Domain.Interfaces;
using OnlineBookRental.Infrastructure.Data;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace OnlineBookRental.Infrastructure.Repositories
{
    // Generic implementation of the IRepository interface for common CRUD operations.
    // T represents the entity type.
    public class GenericRepository<T> : IRepository<T> where T : class
    {
        // The database context instance.
        protected readonly ApplicationDbContext _context;
        // The DbSet for the specific entity type.
        protected readonly DbSet<T> _dbSet;

        // Constructor to initialize the context and DbSet.
        public GenericRepository(ApplicationDbContext context)
        {
            _context = context;
            _dbSet = _context.Set<T>();
        }

        // Adds a new entity to the DbSet.
        public async Task AddAsync(T entity)
        {
            await _dbSet.AddAsync(entity);
        }

        // Deletes an entity by its ID.
        public async Task DeleteAsync(int id)
        {
            // First, find the entity.
            var entity = await GetByIdAsync(id);
            if (entity != null)
            {
                // If found, remove it from the DbSet.
                _dbSet.Remove(entity);
            }
        }

        // Retrieves all entities from the DbSet.
        public async Task<IEnumerable<T>> GetAllAsync()
        {
            return await _dbSet.ToListAsync();
        }

        // Retrieves a single entity by its ID.
        public async Task<T?> GetByIdAsync(int id)
        {
            // Use FindAsync for primary key lookups, or Where/FirstOrDefault for other properties.
            return await _dbSet.FindAsync(id);
        }

        // Updates an existing entity in the DbSet.
        public Task UpdateAsync(T entity)
        {
            // Mark the entity as modified. EF Core will track changes.
            _dbSet.Update(entity);
            return Task.CompletedTask;
        }
    }
}