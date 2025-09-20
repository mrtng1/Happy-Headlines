// HappyHeadlines.ProfanityService/Services/ProfanityService.cs
using HappyHeadlines.ProfanityService.Data;
using Microsoft.EntityFrameworkCore;

namespace HappyHeadlines.ProfanityService.Services
{
    public class ProfanityService
    {
        private readonly ProfanityDbContext _context;

        public ProfanityService(ProfanityDbContext context)
        {
            _context = context;
        }

        public async Task<bool> ContainsProfanityAsync(string text)
        {
            var words = await _context.ProfanityWords.Select(w => w.Word).ToListAsync();
            foreach (var word in words)
            {
                if (text.Contains(word, StringComparison.OrdinalIgnoreCase))
                    return true;
            }
            return false;
        }

        public void AddWord(ProfanityWord word)
        {
            _context.ProfanityWords.Add(word);
            _context.SaveChanges();
        }
    }
}
