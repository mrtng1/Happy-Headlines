namespace HappyHeadlines.NewsletterService.Contracts
{
    public record ArticlePublished(
     Guid ArticleId,
     string Title,
     string Summary,
     string Url,
     DateTime PublishedAtUtc,
     string[] Tags,
     string Continent // fx "Global", "Europe" ...
 );
}
