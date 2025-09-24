using HappyHeadlines.Core.Entities;
using Microsoft.EntityFrameworkCore;

namespace HappyHeadlines.Infrastructure.Repositories;

public class DraftDbContext : DbContext
{
    public DraftDbContext(DbContextOptions<DraftDbContext> options) : base(options) { }

    public DbSet<Draft> Drafts { get; set; }
}