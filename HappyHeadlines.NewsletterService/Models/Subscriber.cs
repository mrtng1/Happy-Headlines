namespace HappyHeadlines.NewsletterService.Models
{
    public class Subscriber
    {
        public Guid Id { get; set; } = Guid.NewGuid();
        public string Email { get; set; } = default!;
        public bool Confirmed { get; set; }
        public bool WantsImmediate { get; set; }
        public bool WantsDaily { get; set; }
        public string[] Categories { get; set; } = Array.Empty<string>();
        public string Continent { get; set; } = "Global";
        public DateTimeOffset CreatedAt { get; set; } = DateTimeOffset.UtcNow;
        public string? ConfirmationToken { get; set; }
    }
}
