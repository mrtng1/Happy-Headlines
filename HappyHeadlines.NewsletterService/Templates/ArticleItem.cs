namespace HappyHeadlines.NewsletterService.Templates
{
    public class ArticleItem
    {
        public Guid Id { get; set; }
        public string Title { get; set; } = "";
        public string Summary { get; set; } = "";
        public string Url { get; set; } = "";
        public DateTime PublishedAtUtc { get; set; }
        public string Continent { get; set; } = "Global";
        public string[] Tags { get; set; } = Array.Empty<string>();
    }
}
