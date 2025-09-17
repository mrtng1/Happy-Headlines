using HappyHeadlines.Core.Entities;
using Microsoft.EntityFrameworkCore;

namespace HappyHeadlines.Infrastructure;

public class ArticleDbContext : DbContext
{
    public ArticleDbContext(DbContextOptions<ArticleDbContext> options) : base(options) { }

    public DbSet<Article> Articles { get; set; }
    
}