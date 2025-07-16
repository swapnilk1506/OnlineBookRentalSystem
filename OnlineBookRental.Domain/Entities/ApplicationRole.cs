using Microsoft.AspNetCore.Identity; // Required for IdentityRole

namespace OnlineBookRental.Domain.Entities
{
    // Custom ApplicationRole class, extending IdentityRole if you need custom role properties.
    public class ApplicationRole : IdentityRole
    {
        // You can add custom properties to your roles here if needed.
        // For example: public string Description { get; set; }
    }
}