using Microsoft.EntityFrameworkCore;
using OnlineBookRental.Application.Services;
using OnlineBookRental.Domain.Interfaces;
using OnlineBookRental.Infrastructure;
using OnlineBookRental.Infrastructure.Data;
using OnlineBookRental.Infrastructure.Repositories;
using OnlineBookRental.Domain.Entities; // NEW: For ApplicationUser and ApplicationRole
using Microsoft.AspNetCore.Identity; // NEW: For Identity configuration

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.

// Configure Entity Framework Core with SQL Server.
// Reads the connection string from appsettings.json.
builder.Services.AddDbContext<ApplicationDbContext>(options =>
    options.UseSqlServer(builder.Configuration.GetConnectionString("DefaultConnection")));

// NEW: Configure ASP.NET Core Identity.
// It uses ApplicationUser for user accounts and ApplicationRole for roles.
// It uses Entity Framework Core for storage and adds default token providers.
builder.Services.AddDefaultIdentity<ApplicationUser>(options => options.SignIn.RequireConfirmedAccount = true)
    .AddRoles<ApplicationRole>() // Enable roles
    .AddEntityFrameworkStores<ApplicationDbContext>(); // Use ApplicationDbContext for Identity storage

// Register our custom services and repositories for Dependency Injection.
// We use Scoped lifetime, meaning a new instance is created per HTTP request.
builder.Services.AddScoped(typeof(IRepository<>), typeof(GenericRepository<>)); // Generic repository
// REMOVED: builder.Services.AddScoped<IBookRepository, BookRepository>(); // Specific book repository registration removed
builder.Services.AddScoped<IUnitOfWork, UnitOfWork>(); // Unit of Work
builder.Services.AddScoped<IBookService, BookService>(); // Book Service

builder.Services.AddControllersWithViews();

// NEW: Add Razor Pages for Identity UI scaffolding.
builder.Services.AddRazorPages();

var app = builder.Build();

// Configure the HTTP request pipeline.
if (!app.Environment.IsDevelopment())
{
    app.UseExceptionHandler("/Home/Error");
    // The default HSTS value is 30 days. You may want to change this for production scenarios, see https://aka.ms/aspnetcore-hsts.
    app.UseHsts();
}

app.UseHttpsRedirection();
app.UseStaticFiles(); // Enables serving static files like CSS, JS, images.

app.UseRouting();

// IMPORTANT: Authentication middleware MUST come before Authorization middleware.
app.UseAuthentication(); // NEW: Enables authentication services.
app.UseAuthorization(); // Enables authorization services.

// Maps incoming requests to controller actions.
app.MapControllerRoute(
    name: "default",
    pattern: "{controller=Home}/{action=Index}/{id?}");

// NEW: Map Razor Pages routes, essential for Identity UI.
app.MapRazorPages();

// Apply database migrations on startup.
// This ensures your database is created/updated when the application starts.
using (var scope = app.Services.CreateScope())
{
    var services = scope.ServiceProvider;
    try
    {
        var context = services.GetRequiredService<ApplicationDbContext>();
        context.Database.Migrate(); // Applies any pending migrations.
    }
    catch (Exception ex)
    {
        var logger = services.GetRequiredService<ILogger<Program>>();
        logger.LogError(ex, "An error occurred while migrating the database.");
    }
}

app.Run();