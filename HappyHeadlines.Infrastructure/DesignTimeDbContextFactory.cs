using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Design;
using Microsoft.Extensions.Configuration;
using System.IO;

namespace HappyHeadlines.Infrastructure;

/// <summary>
/// only used by the EF Core command-line tools at design time to create
/// </summary>
public class DesignTimeArticleDbContextFactory : IDesignTimeDbContextFactory<AppDbContext>
{
    public AppDbContext CreateDbContext(string[] args)
    {
        string path = Path.Combine(Directory.GetCurrentDirectory(), "..", "HappyHeadlines.ArticleService");
        
        IConfigurationRoot configuration = new ConfigurationBuilder()
            .SetBasePath(path)
            .AddJsonFile("appsettings.json")
            .Build();

        var builder = new DbContextOptionsBuilder<AppDbContext>();
        
        var connectionString = configuration.GetConnectionString("Global");

        builder.UseNpgsql(connectionString);

        return new AppDbContext(builder.Options);
    }
}