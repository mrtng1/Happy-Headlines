using HappyHeadlines.ArticleService.Entities;

namespace HappyHeadlines.ArticleService.Interfaces;

public interface IArticleRepository
{
    Task<List<Article>> GetAll(Continent continent, int pageNumber = 1, int pageSize = 10);
    Task<List<Article>> GetAllByDate(DateTime date, Continent continent, int pageNumber = 1, int pageSize = 10);
    Task<Article> Create(Article newArticle);
    Task<bool> Delete(Guid id, Continent continent);
    Task<Article?> GetById(Guid id, Continent continent);
    Task<Article?> Update(Guid id, Article updatedDraft);
}