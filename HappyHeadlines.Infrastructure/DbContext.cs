using HappyHeadlines.Core.Entities;
using Microsoft.EntityFrameworkCore;

namespace HappyHeadlines.Infrastructure;

public class AppDbContext : DbContext
{
    public AppDbContext(DbContextOptions<AppDbContext> options) : base(options) { }

    public DbSet<Article> Articles { get; set; }
    public DbSet<Draft> Drafts { get; set; }
    
}