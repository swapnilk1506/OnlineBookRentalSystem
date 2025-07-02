using System.Collections.Generic;
using System.Threading.Tasks;

namespace OnlineBookRental.Domain.Interfaces
{
    // A generic repository interface defining common CRUD operations.
    // T represents the entity type.
    public interface IRepository<T> where T : class
    {
        // Retrieves an entity by its unique identifier.
        Task<T?> GetByIdAsync(int id);
        // Retrieves all entities of a given type.
        Task<IEnumerable<T>> GetAllAsync();
        // Adds a new entity to the data store.
        Task AddAsync(T entity);
        // Updates an existing entity in the data store.
        Task UpdateAsync(T entity);
        // Deletes an entity from the data store by its unique identifier.
        Task DeleteAsync(int id);
    }
}