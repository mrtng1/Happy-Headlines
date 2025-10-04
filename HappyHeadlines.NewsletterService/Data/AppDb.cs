using HappyHeadlines.NewsletterService.Models;
using Microsoft.EntityFrameworkCore;
using System.Collections.Generic;
using System.Reflection.Emit;

namespace HappyHeadlines.NewsletterService.Data
{
    public class AppDb : DbContext
    {
        public AppDb(DbContextOptions<AppDb> o) : base(o) { }
        public DbSet<Subscriber> Subscribers => Set<Subscriber>();
        public DbSet<Delivery> Deliveries => Set<Delivery>();
        public DbSet<Digest> Digests => Set<Digest>();

        protected override void OnModelCreating(ModelBuilder b)
        {
            b.Entity<Subscriber>().HasIndex(x => x.Email).IsUnique();
            b.Entity<Subscriber>().Property(x => x.Categories)
                .HasConversion(
                    v => string.Join(",", v),
                    v => v.Length == 0 ? Array.Empty<string>() : v.Split(',', StringSplitOptions.RemoveEmptyEntries));

            // Idempotens: én mail pr. (email, articleId, type)
            b.Entity<Delivery>().HasIndex(x => new { x.Email, x.ArticleId, x.Type }).IsUnique();
            // Digest: én pr. (email, date)
            b.Entity<Delivery>().HasIndex(x => new { x.Email, x.DigestDate, x.Type }).IsUnique();
        }
    }
}
