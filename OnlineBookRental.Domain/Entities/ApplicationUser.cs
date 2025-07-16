using Microsoft.AspNetCore.Identity; // Required for IdentityUser

namespace OnlineBookRental.Domain.Entities
{
    // Custom ApplicationUser class, extending IdentityUser to add custom properties.
    public class ApplicationUser : IdentityUser
    {
        // Additional properties for your user.
        public string? FirstName { get; set; }
        public string? LastName { get; set; }
        // You can add more properties here, e.g., Address, City, etc.
    }
}