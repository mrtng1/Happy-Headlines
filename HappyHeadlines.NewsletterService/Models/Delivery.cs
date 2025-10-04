namespace HappyHeadlines.NewsletterService.Models
{
    public class Delivery
    {
        public Guid Id { get; set; } = Guid.NewGuid();
        public string Email { get; set; } = default!;
        public Guid? ArticleId { get; set; } // null for digest
        public string Type { get; set; } = "Immediate"; // or Digest
        public DateOnly? DigestDate { get; set; }
        public DateTimeOffset SentAt { get; set; }
        public string Status { get; set; } = "sent";
    }
}
