using System.Text.Json;
using HappyHeadlines.ArticleService.Entities;
using HappyHeadlines.ArticleService.Interfaces;
using StackExchange.Redis;

namespace HappyHeadlines.ArticleService.Infrastructure;

public class CachingArticleRepository : IArticleRepository
{
    private readonly ArticleRepository _decorated;
    private readonly IDatabase _cache;
    private readonly TimeSpan _cacheDuration = TimeSpan.FromMinutes(5);

    public CachingArticleRepository(ArticleRepository decorated, IConnectionMultiplexer redis)
    {
        _decorated = decorated;
        _cache = redis.GetDatabase();
    }

    public async Task<List<Article>> GetAll(Continent continent, int pageNumber = 1, int pageSize = 30)
    {
        string cacheKey = $"articles:{continent}:page:{pageNumber}:size:{pageSize}";
        var cachedArticles = await _cache.StringGetAsync(cacheKey);

        if (cachedArticles.HasValue)
        {
            // Cache Hit
            return JsonSerializer.Deserialize<List<Article>>(cachedArticles!) ?? new List<Article>();
        }

        // Cache Miss
        var articles = await _decorated.GetAll(continent, pageNumber, pageSize);

        if (articles.Any())
        {
            await _cache.StringSetAsync(cacheKey, JsonSerializer.Serialize(articles), _cacheDuration);
        }

        return articles;
    }

    public async Task<Article?> GetById(Guid id, Continent continent)
    {
        string cacheKey = $"article:{continent}:{id}";
        var cachedArticle = await _cache.StringGetAsync(cacheKey);

        if (cachedArticle.HasValue)
        {
            return JsonSerializer.Deserialize<Article>(cachedArticle!);
        }

        var article = await _decorated.GetById(id, continent);

        if (article != null)
        {
            await _cache.StringSetAsync(cacheKey, JsonSerializer.Serialize(article), _cacheDuration);
        }

        return article;
    }

    public async Task<Article> Create(Article newArticle)
    {
        var createdArticle = await _decorated.Create(newArticle);
        // After creating, the old cache for lists is invalid, but we'll let it expire naturally.
        // We can add the new article to the cache immediately.
        string cacheKey = $"article:{createdArticle.Continent}:{createdArticle.Id}";
        await _cache.StringSetAsync(cacheKey, JsonSerializer.Serialize(createdArticle), _cacheDuration);
        return createdArticle;
    }

    public async Task<bool> Delete(Guid id, Continent continent)
    {
        var deleted = await _decorated.Delete(id, continent);
        if (deleted)
        {
            // When an article is deleted, remove it from the cache.
            string cacheKey = $"article:{continent}:{id}";
            await _cache.KeyDeleteAsync(cacheKey);
        }
        return deleted;
    }
    
    public async Task<Article?> Update(Guid id, Article updatedDraft)
    {
        var updatedArticle = await _decorated.Update(id, updatedDraft);
        if(updatedArticle != null)
        {
            // After updating, remove the old version from cache.
            // The new version will be cached on the next GetById call.
            string cacheKey = $"article:{updatedArticle.Continent}:{id}";
            await _cache.KeyDeleteAsync(cacheKey);
        }
        return updatedArticle;
    }

    public Task<List<Article>> GetAllByDate(DateTime date, Continent continent, int pageNumber = 1, int pageSize = 10)
    {
        // For simplicity, we bypass the cache for this specific query.
        return _decorated.GetAllByDate(date, continent, pageNumber, pageSize);
    }
}