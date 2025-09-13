using HappyHeadlines.Core.DTOs;
using HappyHeadlines.Core.Entities;

namespace HappyHeadlines.Core.Interfaces;

public interface IArticleRepository
{
    Task<IEnumerable<Article>> GetAllArticles();
    Task<Article?> GetArticleById(Guid id);
    Task<Article> CreateArticle(CreateArticleRequest request);
    Task<Article?> UpdateArticle(Guid id, UpdateArticleRequest request);
    Task<bool> DeleteArticle(Guid id);
}