using HappyHeadlines.Core.DTOs;
using HappyHeadlines.Core.Entities;
using HappyHeadlines.Core.Interfaces;

namespace HappyHeadlines.Infrastructure.Repositories;

public class ArticleRepository : IArticleRepository
{
    private readonly ArticleDbContextFactory _contextFactory;

    public ArticleRepository(ArticleDbContextFactory contextFactory)
    {
        _contextFactory = contextFactory;
    }
    
    public async Task<Article> CreateArticle(CreateArticleRequest request)
    {
        using ArticleDbContext context = _contextFactory.Create(request.Continent);
    
        Article newArticle = new Article()
        {
            Title = request.Title,
            Content = request.Content,
            Author = request.Author,
            PublishedAt = DateTime.UtcNow,
            Continent = request.Continent
        };
    
        newArticle = context.Articles.Add(newArticle).Entity;
    
        await context.SaveChangesAsync();
    
        return newArticle;
    }


    public Task<bool> DeleteArticle(Guid id)
    {
        throw new NotImplementedException();
    }

    public Task<IEnumerable<Article>> GetAllArticles()
    {
        throw new NotImplementedException();
    }

    public Task<Article?> GetArticleById(Guid id)
    {
        throw new NotImplementedException();
    }

    public Task<Article?> UpdateArticle(Guid id, UpdateArticleRequest request)
    {
        throw new NotImplementedException();
    }
    
}