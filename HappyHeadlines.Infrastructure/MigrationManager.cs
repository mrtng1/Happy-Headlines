using HappyHeadlines.Infrastructure;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Configuration;
using System;
using System.Linq;
using Microsoft.AspNetCore.Builder;

namespace HappyHeadlines.Infrastructure;

public static class MigrationManager
{
    public static IApplicationBuilder ApplyMigrations(this IApplicationBuilder app)
    {
        using var scope = app.ApplicationServices.CreateScope();
        var configuration = scope.ServiceProvider.GetRequiredService<IConfiguration>();
        var connectionStrings = configuration.GetSection("ConnectionStrings").GetChildren().ToList();

        Console.WriteLine($"Found {connectionStrings.Count} databases to migrate.");
        Console.WriteLine("---");

        foreach (var connectionStringSection in connectionStrings)
        {
            var dbName = connectionStringSection.Key;
            try
            {
                Console.WriteLine($"Attempting to migrate database: '{dbName}'...");

                var options = new DbContextOptionsBuilder<AppDbContext>()
                    .UseNpgsql(connectionStringSection.Value)
                    .Options;

                using var dbContext = new AppDbContext(options);
                dbContext.Database.Migrate();

                Console.WriteLine($"Database '{dbName}' is up to date.");
            }
            catch (Exception ex)
            {
                Console.WriteLine($"An error occurred while migrating database '{dbName}':");
                Console.WriteLine(ex.Message);
            }
            finally
            {
                Console.WriteLine("---");
            }
        }
        return app;
    }
}