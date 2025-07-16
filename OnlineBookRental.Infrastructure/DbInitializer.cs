using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using OnlineBookRental.Domain.Entities;
using OnlineBookRental.Infrastructure.Data;
using OnlineBookRental.Utility;
using System.Threading.Tasks;
using System.Linq; // Required for LINQ operations like AnyAsync
using System; // Required for Guid

namespace OnlineBookRental.Infrastructure
{
    public class DbInitializer
    {
        private readonly UserManager<ApplicationUser> _userManager;
        private readonly RoleManager<ApplicationRole> _roleManager;
        private readonly ApplicationDbContext _db;
        private readonly IPasswordHasher<ApplicationUser> _passwordHasher; // For hashing passwords manually

        public DbInitializer(
            UserManager<ApplicationUser> userManager,
            RoleManager<ApplicationRole> roleManager,
            ApplicationDbContext db,
            IPasswordHasher<ApplicationUser> passwordHasher) // Inject IPasswordHasher
        {
            _userManager = userManager;
            _roleManager = roleManager;
            _db = db;
            _passwordHasher = passwordHasher; // Assign the injected password hasher
        }

        public async Task Initialize()
        {
            // IMPORTANT: Migrations are applied in Program.cs *before* this is called.

            // 1. Apply pending migrations if any (redundant if Program.cs does it, but safe fallback)
            try
            {
                if (_db.Database.GetPendingMigrations().Any()) // Use Any() instead of Count() > 0 for efficiency
                {
                    _db.Database.Migrate();
                }
            }
            catch (Exception)
            {
                // Catch any migration errors here if needed, but let the main Program.cs handle it.
            }

            // 2. Create Roles if they don't exist using parameterized SQL insert
            string adminRoleId = string.Empty;
            string customerRoleId = string.Empty;

            if (!await _roleManager.RoleExistsAsync(SD.Role_Admin))
            {
                adminRoleId = Guid.NewGuid().ToString();
                string adminConcurrencyStamp = Guid.NewGuid().ToString();
                await _db.Database.ExecuteSqlRawAsync(
                    "INSERT INTO AspNetRoles (Id, Name, NormalizedName, ConcurrencyStamp) VALUES ({0}, {1}, {2}, {3})",
                    adminRoleId, SD.Role_Admin, SD.Role_Admin.ToUpper(), adminConcurrencyStamp
                );
            }
            else
            {
                adminRoleId = (await _roleManager.FindByNameAsync(SD.Role_Admin))?.Id ?? Guid.NewGuid().ToString();
            }

            if (!await _roleManager.RoleExistsAsync(SD.Role_Customer))
            {
                customerRoleId = Guid.NewGuid().ToString();
                string customerConcurrencyStamp = Guid.NewGuid().ToString();
                await _db.Database.ExecuteSqlRawAsync(
                    "INSERT INTO AspNetRoles (Id, Name, NormalizedName, ConcurrencyStamp) VALUES ({0}, {1}, {2}, {3})",
                    customerRoleId, SD.Role_Customer, SD.Role_Customer.ToUpper(), customerConcurrencyStamp
                );
            }
            else
            {
                customerRoleId = (await _roleManager.FindByNameAsync(SD.Role_Customer))?.Id ?? Guid.NewGuid().ToString();
            }


            // 3. Create Admin User if it doesn't exist using parameterized SQL insert
            if (!await _userManager.Users.AnyAsync(u => u.Email == "admin@onlinebookrental.com"))
            {
                var adminUser = new ApplicationUser
                {
                    Id = Guid.NewGuid().ToString(),
                    UserName = "admin@onlinebookrental.com",
                    NormalizedUserName = "ADMIN@ONLINEBOOKRENTAL.COM",
                    Email = "admin@onlinebookrental.com",
                    NormalizedEmail = "ADMIN@ONLINEBOOKRENTAL.COM", // Corrected: Use NormalizedEmail property
                    EmailConfirmed = true,
                    FirstName = "System",
                    LastName = "Admin",
                    SecurityStamp = Guid.NewGuid().ToString(),
                    ConcurrencyStamp = Guid.NewGuid().ToString(),
                    PhoneNumber = null,
                    PhoneNumberConfirmed = false,
                    TwoFactorEnabled = false,
                    LockoutEnd = null,
                    LockoutEnabled = false,
                    AccessFailedCount = 0
                };

                adminUser.PasswordHash = _passwordHasher.HashPassword(adminUser, "Admin@123");

                await _db.Database.ExecuteSqlRawAsync(
                    "INSERT INTO AspNetUsers (" +
                    "Id, UserName, NormalizedUserName, Email, NormalizedEmail, EmailConfirmed, " +
                    "PasswordHash, SecurityStamp, ConcurrencyStamp, FirstName, LastName, " +
                    "PhoneNumber, PhoneNumberConfirmed, TwoFactorEnabled, LockoutEnd, LockoutEnabled, AccessFailedCount" +
                    ") VALUES (" +
                    "{0}, {1}, {2}, {3}, {4}, {5}, {6}, {7}, {8}, {9}, {10}, {11}, {12}, {13}, {14}, {15}, {16})",
                    adminUser.Id,
                    adminUser.UserName,
                    adminUser.NormalizedUserName,
                    adminUser.Email,
                    adminUser.NormalizedEmail, // Corrected parameter here
                    adminUser.EmailConfirmed, // bool will be converted to 1/0 by EF Core
                    adminUser.PasswordHash,
                    adminUser.SecurityStamp,
                    adminUser.ConcurrencyStamp,
                    adminUser.FirstName,
                    adminUser.LastName,
                    adminUser.PhoneNumber,
                    adminUser.PhoneNumberConfirmed,
                    adminUser.TwoFactorEnabled,
                    adminUser.LockoutEnd,
                    adminUser.LockoutEnabled,
                    adminUser.AccessFailedCount
                );

                await _db.Database.ExecuteSqlRawAsync(
                    "INSERT INTO AspNetUserRoles (UserId, RoleId) VALUES ({0}, {1})",
                    adminUser.Id, adminRoleId
                );
            }
        }
    }
}