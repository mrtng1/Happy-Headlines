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

    public ArticleDbContext Create(Continent continent)
    {
        string name = Enum.GetName(typeof(Continent), continent) ?? "Europe";
        var connection = _config.GetConnectionString(name) ?? _config.GetConnectionString("Europe");

        var options = new DbContextOptionsBuilder<ArticleDbContext>()
            .UseNpgsql(connection)
            .Options;

        return new ArticleDbContext(options);
    }
}