namespace HappyHeadlines.NewsletterService.Models
{
    public class Digest
    {
        public Guid Id { get; set; } = Guid.NewGuid();
        public string Email { get; set; } = default!;
        public DateOnly Date { get; set; }          // “dagen”
        public List<Guid> ArticleIds { get; set; } = new();
        public DateTimeOffset SentAt { get; set; }
    }
}
