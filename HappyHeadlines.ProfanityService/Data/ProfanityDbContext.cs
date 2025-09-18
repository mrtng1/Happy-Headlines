// HappyHeadlines.ProfanityService/Data/ProfanityDbContext.cs
using Microsoft.EntityFrameworkCore;

namespace HappyHeadlines.ProfanityService.Data
{
    public class ProfanityDbContext : DbContext
    {
        public ProfanityDbContext(DbContextOptions<ProfanityDbContext> options) : base(options) { }

        public DbSet<ProfanityWord> ProfanityWords { get; set; }
    }

    public class ProfanityWord
    {
        public int Id { get; set; }
        public string Word { get; set; }
    }
}
