using HappyHeadlines.ArticleService.Entities;
using HappyHeadlines.ArticleService.Infrastructure;
using Microsoft.EntityFrameworkCore;

namespace HappyHeadlines.ArticleService.Infrastructure;

public class ArticleDbContextFactory
{
    private readonly IConfiguration _config;

    public ArticleDbContextFactory(IConfiguration config)
    {
        _config = config;
    }

    public ArticleDbContext Create(Continent continent)
    {
        string name = Enum.GetName(typeof(Continent), continent) ?? "Global";
        var connection = _config.GetConnectionString(name) ?? _config.GetConnectionString("Global");

        var options = new DbContextOptionsBuilder<ArticleDbContext>()
            .UseNpgsql(connection)
            .Options;

        return new ArticleDbContext(options);
    }
}