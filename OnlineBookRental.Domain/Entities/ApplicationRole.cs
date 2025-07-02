using Microsoft.AspNetCore.Identity; // Required for IdentityRole inheritance

namespace OnlineBookRental.Domain.Entities
{
    // Represents a role in the application. Inherits from IdentityRole<string>
    // to specify string as the key type, matching IdentityDbContext's expectation.
    public class ApplicationRole : IdentityRole<string> // CRITICAL FIX: Inherit from IdentityRole<string>
    {
        // Add any additional properties specific to your application role here if needed.
        // For example:
        // public string Description { get; set; }
    }
}