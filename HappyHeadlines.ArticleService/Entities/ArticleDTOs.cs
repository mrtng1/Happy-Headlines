using HappyHeadlines.ArticleService.Entities;
using HappyHeadlines.ArticleService.Entities;

namespace HappyHeadlines.ArticleService.DTOs;

public record ArticleDto(Guid Id, string Title, string Content, string Author, DateTime PublishedAt, Continent Continent);
public record CreateArticleRequest(string Title, string Content, string Author, Continent Continent);
public record UpdateArticleRequest(string Title, string Content, string Author, Continent Continent);