using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using OnlineBookRental.Domain.Entities;
using OnlineBookRental.Domain.Interfaces;
using OnlineBookRental.Infrastructure.Data; // Need ApplicationDbContext for direct query
using System.Linq; // For LINQ methods like Where, ToListAsync
using System.Threading; // For CancellationToken
using System.Threading.Tasks; // For Task
using System;
using Microsoft.EntityFrameworkCore; // For DateTime, TimeSpan, Timer

namespace OnlineBookRental.Application.Services
{
    /// <summary>
    /// A background service that periodically checks for "Pending" rental headers
    /// that have exceeded a defined timeout and returns the associated book quantity
    /// to the inventory, marking the rental as "Expired".
    /// </summary>
    public class ExpiredRentalCleanupService : IHostedService, IDisposable
    {
        private readonly ILogger<ExpiredRentalCleanupService> _logger;
        private readonly IServiceScopeFactory _scopeFactory; // Used to create isolated service scopes
        private Timer? _timer = null; // Timer to schedule periodic execution

        // Configuration for cleanup intervals and timeouts
        private const int CleanupIntervalMinutes = 5; // How often the service runs its check (every 5 minutes)
        private const int PendingRentalTimeoutMinutes = 15; // How long a rental can stay "Pending" before expiring

        /// <summary>
        /// Constructor for the ExpiredRentalCleanupService.
        /// </summary>
        /// <param name="logger">Logger for logging service activities.</param>
        /// <param name="scopeFactory">Factory to create service scopes for database operations.</param>
        public ExpiredRentalCleanupService(ILogger<ExpiredRentalCleanupService> logger, IServiceScopeFactory scopeFactory)
        {
            _logger = logger;
            _scopeFactory = scopeFactory;
        }

        /// <summary>
        /// Starts the background service. Initializes and starts the timer.
        /// </summary>
        /// <param name="cancellationToken">Cancellation token to signal when the service should stop.</param>
        public Task StartAsync(CancellationToken cancellationToken)
        {
            _logger.LogInformation("Expired Rental Cleanup Service running.");

            // Schedule DoWork to run immediately and then every CleanupIntervalMinutes
            _timer = new Timer(DoWork, null, TimeSpan.Zero, TimeSpan.FromMinutes(CleanupIntervalMinutes));

            return Task.CompletedTask;
        }

        /// <summary>
        /// The core work method executed by the timer.
        /// Checks for and processes expired pending rentals.
        /// </summary>
        /// <param name="state">State object (not used in this case).</param>
        private async void DoWork(object? state)
        {
            _logger.LogInformation("Expired Rental Cleanup Service is checking for expired rentals.");

            // Create a new scope for each execution to ensure services (like DbContext) are correctly scoped
            using (var scope = _scopeFactory.CreateScope())
            {
                // Get required services from the newly created scope
                var context = scope.ServiceProvider.GetRequiredService<ApplicationDbContext>();
                var unitOfWork = scope.ServiceProvider.GetRequiredService<IUnitOfWork>();

                try
                {
                    // Calculate the cutoff time: any rental initiated before this time is considered expired
                    var cutoffTime = DateTime.Now.AddMinutes(-PendingRentalTimeoutMinutes);

                    // Query for pending rentals that are older than the cutoff time
                    // Eagerly load RentalDetails and Book to avoid N+1 queries
                    var expiredPendingRentals = await context.RentalHeaders
                        .Where(rh => rh.RentalStatus == "Pending" && rh.RentalDate < cutoffTime)
                        .Include(rh => rh.RentalDetails) // Include the collection of rental details
                            .ThenInclude(rd => rd.Book) // Then include the Book for each rental detail
                        .ToListAsync();

                    _logger.LogInformation($"Found {expiredPendingRentals.Count} expired pending rentals to process.");

                    // Process each expired rental
                    foreach (var rentalHeader in expiredPendingRentals)
                    {
                        foreach (var rentalDetail in rentalHeader.RentalDetails)
                        {
                            var book = rentalDetail.Book;
                            if (book != null)
                            {
                                // Return the quantity to the book inventory
                                book.QuantityAvailable++;
                                // Update the book entity via UnitOfWork
                                await unitOfWork.Books.UpdateAsync(book);
                                _logger.LogInformation($"Returned quantity for Book ID: {book.Id} ({book.Title}). New quantity: {book.QuantityAvailable}");
                            }
                        }

                        // Update the rental header status to "Expired"
                        rentalHeader.RentalStatus = "Expired";
                        // Update the rental header entity via UnitOfWork
                        await unitOfWork.RentalHeaders.UpdateAsync(rentalHeader);
                        _logger.LogInformation($"Updated RentalHeader ID: {rentalHeader.Id} to status 'Expired'.");
                    }

                    // Save all changes made in this batch to the database
                    await unitOfWork.CompleteAsync();
                    _logger.LogInformation("Expired rental cleanup completed for this cycle.");
                }
                catch (Exception ex)
                {
                    // Log any errors that occur during the cleanup process
                    _logger.LogError(ex, "Error occurred while cleaning up expired rentals.");
                }
            }
        }

        /// <summary>
        /// Stops the background service. Stops the timer.
        /// </summary>
        /// <param name="cancellationToken">Cancellation token to signal when the service should stop.</param>
        public Task StopAsync(CancellationToken cancellationToken)
        {
            _logger.LogInformation("Expired Rental Cleanup Service is stopping.");
            // Stop the timer from firing any more callbacks
            _timer?.Change(Timeout.Infinite, 0);
            return Task.CompletedTask;
        }

        /// <summary>
        /// Disposes the timer when the service is no longer needed.
        /// </summary>
        public void Dispose()
        {
            _timer?.Dispose();
        }
    }
}