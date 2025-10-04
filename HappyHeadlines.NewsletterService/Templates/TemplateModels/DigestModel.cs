namespace HappyHeadlines.NewsletterService.Templates.TemplateModels
{
    public class DigestModel
    {
        public DateOnly Date { get; set; }
        public string Continent { get; set; } = "Global";
        public IReadOnlyList<ArticleItem> Articles { get; set; } = Array.Empty<ArticleItem>();
        public string? UnsubscribeUrl { get; set; }
    }
}
