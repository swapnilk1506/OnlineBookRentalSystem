using Microsoft.AspNetCore.Identity.EntityFrameworkCore; // Required for IdentityDbContext
using Microsoft.EntityFrameworkCore;
using OnlineBookRental.Domain.Entities; // Ensure this is present for ApplicationUser, ApplicationRole, Book, RentalHeader, RentalDetail

namespace OnlineBookRental.Infrastructure.Data
{
    // The main database context for the application, inheriting from IdentityDbContext.
    // It uses ApplicationUser for user accounts and ApplicationRole for roles, both with string primary keys.
    public class ApplicationDbContext : IdentityDbContext<ApplicationUser, ApplicationRole, string>
    {
        // Constructor that accepts DbContextOptions for configuration.
        public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options) : base(options)
        {
        }

        // DbSet for Book entities, representing the Books table in the database.
        public DbSet<Book> Books { get; set; }
        // NEW: DbSet for RentalHeader entities.
        public DbSet<RentalHeader> RentalHeaders { get; set; }
        // NEW: DbSet for RentalDetail entities.
        public DbSet<RentalDetail> RentalDetails { get; set; }


        // Configures the model that was discovered by convention from the given context.
        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            // IMPORTANT: Call base.OnModelCreating(modelBuilder) first for Identity tables.
            // This configures the Identity tables (AspNetUsers, AspNetRoles, etc.)
            base.OnModelCreating(modelBuilder);

            // Explicitly configure primary keys for Identity tables if issues persist.
            modelBuilder.Entity<ApplicationUser>().HasKey(u => u.Id);
            modelBuilder.Entity<ApplicationRole>().HasKey(r => r.Id);

            // Configure precision for RentalPricePerDay to prevent truncation warnings
            modelBuilder.Entity<Book>()
                .Property(b => b.RentalPricePerDay)
                .HasPrecision(18, 2); // 18 total digits, 2 after decimal point

            // Configure precision for RentalHeader TotalRentalAmount
            modelBuilder.Entity<RentalHeader>()
                .Property(rh => rh.TotalRentalAmount)
                .HasPrecision(18, 2);

            // Configure precision for RentalDetail PricePerDay and TotalDetailAmount
            modelBuilder.Entity<RentalDetail>()
                .Property(rd => rd.PricePerDay)
                .HasPrecision(18, 2);
            modelBuilder.Entity<RentalDetail>()
                .Property(rd => rd.TotalDetailAmount)
                .HasPrecision(18, 2);

            // Define relationships for RentalHeader and RentalDetail
            modelBuilder.Entity<RentalHeader>()
                .HasOne(rh => rh.ApplicationUser) // A RentalHeader has one ApplicationUser
                .WithMany() // An ApplicationUser can have many RentalHeaders
                .HasForeignKey(rh => rh.ApplicationUserId) // Foreign key is ApplicationUserId
                .OnDelete(DeleteBehavior.Restrict); // Prevent cascade delete if user is deleted

            modelBuilder.Entity<RentalDetail>()
                .HasOne(rd => rd.RentalHeader) // A RentalDetail has one RentalHeader
                .WithMany(rh => rh.RentalDetails) // A RentalHeader has many RentalDetails
                .HasForeignKey(rd => rd.RentalHeaderId) // Foreign key is RentalHeaderId
                .OnDelete(DeleteBehavior.Cascade); // If RentalHeader is deleted, its details are deleted

            modelBuilder.Entity<RentalDetail>()
                .HasOne(rd => rd.Book) // A RentalDetail has one Book
                .WithMany() // A Book can be in many RentalDetails
                .HasForeignKey(rd => rd.BookId) // Foreign key is BookId
                .OnDelete(DeleteBehavior.Restrict); // Prevent cascade delete if book is deleted

            // Seed initial data for demonstration purposes.
            modelBuilder.Entity<Book>().HasData(
                new Book { Id = 1, Title = "The Great Gatsby", Author = "F. Scott Fitzgerald", ISBN = "978-0743273565", Description = "A novel about the roaring twenties.", ImageUrl = "/images/gatsby.png", RentalPricePerDay = 2.50m, QuantityAvailable = 5 },
                new Book { Id = 2, Title = "To Kill a Mockingbird", Author = "Harper Lee", ISBN = "978-0446310789", Description = "A classic of modern American literature.", ImageUrl = "/images/mockingbird.png", RentalPricePerDay = 3.00m, QuantityAvailable = 3 },
                new Book { Id = 3, Title = "1984", Author = "George Orwell", ISBN = "978-0451524935", Description = "Dystopian social science fiction novel.", ImageUrl = "/images/1984.png", RentalPricePerDay = 2.00m, QuantityAvailable = 8 }
            );
        }
    }
}