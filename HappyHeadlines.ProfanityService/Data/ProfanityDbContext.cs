// HappyHeadlines.ProfanityService/Data/ProfanityDbContext.cs
using Microsoft.EntityFrameworkCore;

namespace HappyHeadlines.ProfanityService.Data
{
    public class ProfanityDbContext : DbContext
    {
        public ProfanityDbContext(DbContextOptions<ProfanityDbContext> options) : base(options) { }

        public DbSet<ProfanityWord> ProfanityWords { get; set; }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.Entity<ProfanityWord>().HasData(
                new ProfanityWord { Id = 1, Word = "badword1" },
                new ProfanityWord { Id = 2, Word = "badword2" },
                new ProfanityWord { Id = 3, Word = "badword3" }
                );
        }
    }

    public class ProfanityWord
    {
        public int Id { get; set; }
        public string Word { get; set; }
    }

    
 }
