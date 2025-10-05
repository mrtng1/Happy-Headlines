namespace HappyHeadlines.PublisherService.DTOs;


public record CreateArticleRequest(string Title, string Content, string Author, int Continent);
