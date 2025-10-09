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
    private readonly ILogger<CachingArticleRepository> _logger;

    public CachingArticleRepository(ArticleRepository decorated, IConnectionMultiplexer redis, ILogger<CachingArticleRepository> logger)
    {
        _decorated = decorated;
        _cache = redis.GetDatabase();
        _logger = logger;
    }

    public async Task<List<Article>> GetAll(Continent continent, int pageNumber = 1, int pageSize = 30)
    {
        string cacheKey = $"articles:{continent}:page:{pageNumber}:size:{pageSize}";
        var cachedArticles = await _cache.StringGetAsync(cacheKey);

        if (cachedArticles.HasValue)
        {
            // Cache Hit
            _logger.LogInformation("CACHE HIT");
            return JsonSerializer.Deserialize<List<Article>>(cachedArticles!) ?? new List<Article>();
        }

        // Cache Miss
        List<Article> articles = await _decorated.GetAll(continent, pageNumber, pageSize);

        // If missed -> save the re-retrieved articles to cache
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
        Article createdArticle = await _decorated.Create(newArticle);
        // After creating, the old cache for lists is invalid, but we'll let it expire naturally.
        // We can add the new article to the cache immediately.
        string cacheKey = $"article:{createdArticle.Continent}:{createdArticle.Id}";
        await _cache.StringSetAsync(cacheKey, JsonSerializer.Serialize(createdArticle), _cacheDuration);
        return createdArticle;
    }

    public async Task<bool> Delete(Guid id, Continent continent)
    {
        bool deleted = await _decorated.Delete(id, continent);
        if (deleted)
        {
            string cacheKey = $"article:{continent}:{id}";
            await _cache.KeyDeleteAsync(cacheKey);
        }
        return deleted;
    }
    
    public async Task<Article?> Update(Guid id, Article updatedDraft)
    {
        Article updatedArticle = await _decorated.Update(id, updatedDraft);
        if(updatedArticle != null)
        {
            string cacheKey = $"article:{updatedArticle.Continent}:{id}";
            await _cache.KeyDeleteAsync(cacheKey);
        }
        return updatedArticle;
    }

    public async Task<List<Article>> GetAllRecent(Continent continent, int pageNumber = 1, int pageSize = 30)
    {
        // this cache is for the global articles ONLY
        // If a different continent is requested -> bypass this cache
        if (continent != Continent.Global)
        {
            _logger.LogInformation("GetAllRecent called for non-global continent {Continent}. Bypassing pre-warmed cache and fetching directly from DB.", continent);
            return await _decorated.GetAllRecent(continent, pageNumber, pageSize);
        }
    
        // write to
        const string cacheKey = "articles:recent_14_days"; 
    
        var cachedArticles = await _cache.StringGetAsync(cacheKey);

        // cache found
        if (cachedArticles.HasValue)
        {
            _logger.LogInformation("PRE-WARMED CACHE HIT: Found recent global articles in the cache using key '{CacheKey}'.", cacheKey);
        
            // Deserialize the full list of articles from the cache
            List<Article> allRecentArticles = JsonSerializer.Deserialize<List<Article>>(cachedArticles!) ?? new List<Article>();
        
            // Perform pagination on the list that's now in memory.
            List<Article> paginatedArticles = allRecentArticles
                .Skip((pageNumber - 1) * pageSize)
                .Take(pageSize)
                .ToList();
            
            return paginatedArticles;
        }

        // No cache found -> log a warning ?
        _logger.LogWarning("PRE-WARMED CACHE MISS: Did not find key '{CacheKey}'. Fetching from database as a fallback.", cacheKey);
        return await _decorated.GetAllRecent(continent, pageNumber, pageSize);
    }
}