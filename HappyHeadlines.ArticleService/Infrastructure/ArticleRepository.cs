using HappyHeadlines.ArticleService.Entities;
using HappyHeadlines.WebApi.Interfaces;
using Microsoft.EntityFrameworkCore;

namespace HappyHeadlines.ArticleService.Infrastructure;

public class ArticleRepository : IArticleRepository
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
    

    public async Task<Article?> GetById(Guid id, Continent continent)
    {
        await using var context = _contextFactory.Create(continent);
        var article = await context.Articles.FindAsync(id);
        if (article != null)
        {
            return article;
        }
        
        return null;
    }

    public async Task<Article?> Update(Guid id, Article updatedDraft)
    {
        await using var context = _contextFactory.Create(updatedDraft.Continent);

        Article? existingArticle = await context.Articles.FindAsync(id);

        if (existingArticle == null)
        {
            return null;
        }

        existingArticle.Title = updatedDraft.Title;
        existingArticle.Content = updatedDraft.Content;
        existingArticle.Author = updatedDraft.Author;
            
        await context.SaveChangesAsync();
        return existingArticle;
    }
    
}