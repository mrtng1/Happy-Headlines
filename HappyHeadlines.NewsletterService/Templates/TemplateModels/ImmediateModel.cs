namespace HappyHeadlines.NewsletterService.Templates.TemplateModels
{
    public class ImmediateModel
    {
        public string Title { get; set; } = "";
        public string Summary { get; set; } = "";
        public string Url { get; set; } = "";
        // optional extras
        public string? Continent { get; set; }
        public DateTime PublishedAtUtc { get; set; }
    }
}
