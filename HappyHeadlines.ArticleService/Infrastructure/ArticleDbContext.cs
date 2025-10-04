using HappyHeadlines.ArticleService.Entities;
using Microsoft.EntityFrameworkCore;

namespace HappyHeadlines.ArticleService.Infrastructure;

public class ArticleDbContext : DbContext
{
    public ArticleDbContext(DbContextOptions<ArticleDbContext> options) : base(options) { }

    public DbSet<Article> Articles { get; set; }
}