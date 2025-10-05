namespace HappyHeadlines.NewsletterService.DTOs;

public record ArticlePublished(
    Guid ArticleId, 
    string Title, 
    string Summary, 
    string Url, 
    DateTime PublishedAtUtc, 
    string Continent, 
    string[] Tags);