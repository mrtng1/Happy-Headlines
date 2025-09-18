using HappyHeadlines.Core.DTOs;
using HappyHeadlines.Core.Entities;
using HappyHeadlines.Core.Interfaces;
using Microsoft.EntityFrameworkCore;

namespace HappyHeadlines.Infrastructure.Repositories;

public class ArticleRepository : IRepository<Article>
{
    private readonly ArticleDbContextFactory _contextFactory;

    public ArticleRepository(ArticleDbContextFactory contextFactory)
    {
        _contextFactory = contextFactory;
    }
    
    public async Task<List<Article>> GetAll(Continent continent, int pageNumber = 1, int pageSize = 10)
    {
        await using var context = _contextFactory.Create(continent);
        return await context.Articles
            .Skip((pageNumber - 1) * pageSize)
            .Take(pageSize)
            .ToListAsync();
    }
    
    public async Task<Article> Create(Article newArticle)
    {
        await using var context = _contextFactory.Create(newArticle.Continent);

        try
        {
            newArticle = context.Articles.Add(newArticle).Entity;
            
            await context.SaveChangesAsync();
        }
        catch (Exception e)
        {
            Console.WriteLine(e);
            throw;
        }
    
        return newArticle;
    }


    public async Task<bool> Delete(Guid id, Continent continent)
    {
        await using var context = _contextFactory.Create(continent);

        Article? article = await context.Articles.FindAsync(id);

        if (article == null)
            return false;

        context.Articles.Remove(article);
        await context.SaveChangesAsync();
        return true;
    }


    public Task<IEnumerable<Article>> GetAll()
    {
        throw new NotImplementedException();
    }

    public Task<Article?> GetById(Guid id)
    {
        throw new NotImplementedException();
    }

    public Task<Article?> Update(Guid id, Article updatedArticle)
    {
        throw new NotImplementedException();
    }
    
}