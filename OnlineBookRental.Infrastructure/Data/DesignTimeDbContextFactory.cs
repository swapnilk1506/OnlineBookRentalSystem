using System;
using System.IO;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Design;
using Microsoft.Extensions.Configuration;


namespace OnlineBookRental.Infrastructure.Data
{
    public class DesignTimeDbContextFactory : IDesignTimeDbContextFactory<ApplicationDbContext>
    {
        public ApplicationDbContext CreateDbContext(string[] args)
        {
            // 1) try to read connection string from environment variable first (good for CI / Render)
            var envConn = Environment.GetEnvironmentVariable("ConnectionStrings__DefaultConnection");
            if (!string.IsNullOrEmpty(envConn))
            {
                var optionsBuilder = new DbContextOptionsBuilder<ApplicationDbContext>();
                optionsBuilder.UseNpgsql(envConn);
                return new ApplicationDbContext(optionsBuilder.Options);
            }

            // 2) Fallback to appsettings.json in solution root (works locally)
            string FindSolutionRootWithAppSettings()
            {
                var dir = new DirectoryInfo(Directory.GetCurrentDirectory());
                for (int i = 0; i < 6 && dir != null; i++)
                {
                    var candidate = Path.Combine(dir.FullName, "appsettings.json");
                    if (File.Exists(candidate))
                        return dir.FullName; // found it
                    dir = dir.Parent;
                }
                return Directory.GetCurrentDirectory(); // fallback if not found
            }

            var solutionRoot = FindSolutionRootWithAppSettings();
            var configBuilder = new ConfigurationBuilder()
                .SetBasePath(solutionRoot)
                .AddJsonFile("appsettings.json", optional: true, reloadOnChange: false)
                .AddJsonFile("appsettings.Development.json", optional: true, reloadOnChange: false)
                .AddEnvironmentVariables();

            var configuration = configBuilder.Build();
            var conn = configuration.GetConnectionString("DefaultConnection");


            if (string.IsNullOrEmpty(conn))
                throw new InvalidOperationException("Could not find a connection string named 'DefaultConnection' for design-time DbContext creation.");

            var options = new DbContextOptionsBuilder<ApplicationDbContext>();
            options.UseNpgsql(conn);

            return new ApplicationDbContext(options.Options);
        }
    }
}
