using HappyHeadlines.CommentService.Entities;
using Microsoft.EntityFrameworkCore;

namespace HappyHeadlines.CommentService.Infrastructure;


    public class CommentDbContext : DbContext
    {
        public CommentDbContext(DbContextOptions<CommentDbContext> options) : base(options)
        {
        }

        public DbSet<Comment> Comments { get; set; }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            // index on article id for faster lookups
            modelBuilder.Entity<Comment>()
                .HasIndex(c => c.ArticleId); 
        }
    }