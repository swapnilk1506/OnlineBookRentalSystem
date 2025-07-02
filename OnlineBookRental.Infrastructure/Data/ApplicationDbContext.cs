using Microsoft.EntityFrameworkCore;
using OnlineBookRental.Domain.Entities;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore; // New import for IdentityDbContext

namespace OnlineBookRental.Infrastructure.Data
{
    // The main database context, now inheriting from IdentityDbContext
    // to include Identity tables for ApplicationUser and ApplicationRole.
    public class ApplicationDbContext : IdentityDbContext<ApplicationUser, ApplicationRole, string> // Updated inheritance
    {
        // Constructor that accepts DbContextOptions for configuration.
        public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options) : base(options)
        {
        }

        // DbSet for Book entities, representing the Books table in the database.
        public DbSet<Book> Books { get; set; }

        // Configures the model that was discovered by convention from the given context.
        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            // Call the base IdentityDbContext's OnModelCreating first.
            // This is crucial for configuring the Identity tables.
            base.OnModelCreating(modelBuilder);

            // Configure precision for RentalPricePerDay to prevent truncation warnings
            modelBuilder.Entity<Book>()
                .Property(b => b.RentalPricePerDay)
                .HasPrecision(18, 2); // 18 total digits, 2 after decimal point

            // Seed initial data for demonstration purposes.
            modelBuilder.Entity<Book>().HasData(
                new Book { Id = 1, Title = "The Great Gatsby", Author = "F. Scott Fitzgerald", ISBN = "978-0743273565", Description = "A novel about the roaring twenties.", ImageUrl = "https://placehold.co/150x200/F0F0F0/000?text=Gatsby", RentalPricePerDay = 2.50m, QuantityAvailable = 5 },
                new Book { Id = 2, Title = "To Kill a Mockingbird", Author = "Harper Lee", ISBN = "978-0446310789", Description = "A classic of modern American literature.", ImageUrl = "https://placehold.co/150x200/F0F0F0/000?text=Mockingbird", RentalPricePerDay = 3.00m, QuantityAvailable = 3 },
                new Book { Id = 3, Title = "1984", Author = "George Orwell", ISBN = "978-0451524935", Description = "Dystopian social science fiction novel.", ImageUrl = "https://placehold.co/150x200/F0F0F0/000?text=1984", RentalPricePerDay = 2.00m, QuantityAvailable = 8 }
            );

            // Optional: Customize Identity table names if you don't want the default AspNet prefix
            // foreach (var entityType in modelBuilder.Model.GetEntityTypes())
            // {
            //     var tableName = entityType.GetTableName();
            //     if (tableName.StartsWith("AspNet"))
            //     {
            //         entityType.SetTableName(tableName.Substring(6));
            //     }
            // }
        }
    }
}