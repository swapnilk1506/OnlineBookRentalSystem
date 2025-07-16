using Microsoft.EntityFrameworkCore;
using OnlineBookRental.Application.Services;
using OnlineBookRental.Domain.Entities; // Required for ApplicationUser and ApplicationRole
using OnlineBookRental.Domain.Interfaces;
using OnlineBookRental.Infrastructure;
using OnlineBookRental.Infrastructure.Data;
using OnlineBookRental.Infrastructure.Repositories;
using Microsoft.AspNetCore.Identity; // Required for Identity
using Microsoft.Extensions.Hosting; // Required for IHostedService

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.

// Configure Entity Framework Core with SQL Server.
// Reads the connection string from appsettings.json.
builder.Services.AddDbContext<ApplicationDbContext>(options =>
    options.UseSqlServer(builder.Configuration.GetConnectionString("DefaultConnection")));

// Configure ASP.NET Core Identity.
// Uses ApplicationUser for user accounts and ApplicationRole for roles.
// IMPORTANT: Set RequireConfirmedAccount to false for development if not setting up email sending.
builder.Services.AddDefaultIdentity<ApplicationUser>(options => options.SignIn.RequireConfirmedAccount = false)
    .AddRoles<ApplicationRole>() // Add role management capabilities
    .AddEntityFrameworkStores<ApplicationDbContext>(); // Use ApplicationDbContext for Identity data

// Register our custom services and repositories for Dependency Injection.
// We use Scoped lifetime, meaning a new instance is created per HTTP request.
builder.Services.AddScoped(typeof(IRepository<>), typeof(GenericRepository<>)); // Generic repository
builder.Services.AddScoped<IUnitOfWork, UnitOfWork>(); // Unit of Work
builder.Services.AddScoped<IBookService, BookService>(); // Book Service
builder.Services.AddScoped<IRentalService, RentalService>(); // Register Rental Service
builder.Services.AddScoped<DbInitializer>(); // Register DbInitializer for seeding

// NEW: Register the background service for cleaning up expired pending rentals
builder.Services.AddHostedService<ExpiredRentalCleanupService>();

builder.Services.AddControllersWithViews();
// Add Razor Pages support, which is needed for Identity UI.
builder.Services.AddRazorPages(); // Keep this here for service registration

var app = builder.Build();

// Configure the HTTP request pipeline.
if (!app.Environment.IsDevelopment())
{
    app.UseExceptionHandler("/Home/Error");
    app.UseHsts();
}

app.UseHttpsRedirection();
app.UseStaticFiles();

app.UseRouting();

// Order is important for authentication and authorization middleware.
app.UseAuthentication();
app.UseAuthorization();

// Explicitly define the route for Rental/ConfirmRental to ensure it's matched first
app.MapControllerRoute(
    name: "rentalConfirmation",
    pattern: "Rental/ConfirmRental/{rentalHeaderId:int}",
    defaults: new { controller = "Rental", action = "ConfirmRental" });

// Default MVC route
app.MapControllerRoute(
    name: "default",
    pattern: "{controller=Home}/{action=Index}/{id?}");

// Map Razor Pages after all specific and default MVC controller routes
app.MapRazorPages();


// Apply database migrations and seed data on startup.
using (var scope = app.Services.CreateScope())
{
    var services = scope.ServiceProvider;
    try
    {
        var dbInitializer = services.GetRequiredService<DbInitializer>();
        await dbInitializer.Initialize();
    }
    catch (Exception ex)
    {
        var logger = services.GetRequiredService<ILogger<Program>>();
        logger.LogError(ex, "An error occurred while migrating or seeding the database.");
    }
}

app.Run();