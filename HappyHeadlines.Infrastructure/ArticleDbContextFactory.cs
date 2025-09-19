using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using HappyHeadlines.Core.Entities;

namespace HappyHeadlines.Infrastructure;

public class ArticleDbContextFactory
{
    private readonly IConfiguration _config;

    public ArticleDbContextFactory(IConfiguration config)
    {
        _config = config;
    }

    public AppDbContext Create(Continent continent)
    {
        string name = Enum.GetName(typeof(Continent), continent) ?? "Global";
        var connection = _config.GetConnectionString(name) ?? _config.GetConnectionString("Global");

        var options = new DbContextOptionsBuilder<AppDbContext>()
            .UseNpgsql(connection)
            .Options;

        return new AppDbContext(options);
    }
}