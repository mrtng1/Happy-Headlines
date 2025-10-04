using System.ComponentModel.DataAnnotations;

namespace HappyHeadlines.NewsletterService.DTOs
{
    public class CreateSubscriptionRequest
    {
        [Required, EmailAddress] public string Email { get; set; } = default!;
        public bool WantsImmediate { get; set; } = true;
        public bool WantsDaily { get; set; } = true;
        public string? Continent { get; set; }
        public string[]? Categories { get; set; }
    }
}
