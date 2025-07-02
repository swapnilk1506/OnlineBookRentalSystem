using Microsoft.AspNetCore.Identity; // Required for IdentityUser inheritance

namespace OnlineBookRental.Domain.Entities
{
    // Represents a user in the application. Inherits from IdentityUser<string>
    // to specify string as the key type, matching IdentityDbContext's expectation.
    public class ApplicationUser : IdentityUser<string> // CRITICAL FIX: Inherit from IdentityUser<string>
    {
        // Add any additional properties specific to your application user here.
        // For example:
        public string? FirstName { get; set; }
        public string? LastName { get; set; }
        // public string Address { get; set; }
        // public DateTime RegistrationDate { get; set; }
    }
}