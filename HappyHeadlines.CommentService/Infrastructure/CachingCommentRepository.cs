using System.Collections.Concurrent;
using System.Text.Json;
using HappyHeadlines.CommentService.Entities;
using HappyHeadlines.CommentService.Interfaces;
using Microsoft.Extensions.Caching.Memory;
using Prometheus;

namespace HappyHeadlines.CommentService.Infrastructure;

public class CachingCommentRepository : ICommentRepository
{
    private readonly ICommentRepository _decorated;
    private readonly IMemoryCache _cache;
    private readonly ILogger<CachingCommentRepository> _logger;
    
    // LRU - caching mechanism
    private static readonly ConcurrentQueue<Guid> LruTracker = new();
    private static readonly ConcurrentDictionary<Guid, bool> LruKeys = new();
    private const int CacheSizeLimit = 30;

    // Metrics
    private static readonly Counter CacheHits = Metrics.CreateCounter("comment_cache_hits_total", "Number of comment cache hits");
    private static readonly Counter CacheMisses = Metrics.CreateCounter("comment_cache_misses_total", "Number of comment cache misses");

    public CachingCommentRepository(ICommentRepository decorated, IMemoryCache cache, ILogger<CachingCommentRepository> logger)
    {
        _decorated = decorated;
        _cache = cache;
        _logger = logger;
    }

    public async Task<List<Comment>> GetCommentsByArticleId(Guid articleId, int page, int pageSize)
    {
        // First, try to get the full list of comments from the cache
        if (_cache.TryGetValue(articleId, out List<Comment>? allComments))
        {
            _logger.LogInformation("CACHE HIT for comments on article {ArticleId}", articleId);
            CacheHits.Inc();
            UpdateLruTracker(articleId);

            if (allComments == null) return new List<Comment>();

            // Cache Hit
            List<Comment> paginatedComments = allComments
                .Skip((page - 1) * pageSize)
                .Take(pageSize)
                .ToList();
        
            return paginatedComments;
        }

        _logger.LogWarning("CACHE MISS for comments on article {ArticleId}. Fetching from database.", articleId);
        CacheMisses.Inc(); 

        // Cache Miss
        List<Comment> commentsFromDb = await _decorated.GetCommentsByArticleId(articleId, 1, int.MaxValue); // Fetch all to be saved in cache

        EvictOldestIfFull();

        MemoryCacheEntryOptions cacheEntryOptions = new MemoryCacheEntryOptions().SetSlidingExpiration(TimeSpan.FromHours(1));
    
        // Store the whole list in the cache
        _cache.Set(articleId, commentsFromDb, cacheEntryOptions);
    
        UpdateLruTracker(articleId);

        // Cache miss
        List<Comment> paginatedResult = commentsFromDb
            .Skip((page - 1) * pageSize)
            .Take(pageSize)
            .ToList();

        return paginatedResult;
    }

    private void UpdateLruTracker(Guid articleId)
    {
        if (LruKeys.TryAdd(articleId, true))
        {
            LruTracker.Enqueue(articleId);
        }
    }

    private void EvictOldestIfFull()
    {
        if (LruKeys.Count >= CacheSizeLimit)
        {
            if (LruTracker.TryDequeue(out Guid oldestArticleId))
            {
                _cache.Remove(oldestArticleId);
                LruKeys.TryRemove(oldestArticleId, out _);
                _logger.LogInformation("Cache full. Evicted comments for LRU article {ArticleId}", oldestArticleId);
            }
        }
    }

    // Pass-through functions without caching
    public Task CreateComment(Comment comment)
    {
        return _decorated.CreateComment(comment);
    }
    
    public Task<Comment?> GetCommentById(Guid id)
    {
        return _decorated.GetCommentById(id);
    }
    
    public Task<bool> DeleteComment(Guid id)
    {
        return _decorated.DeleteComment(id);
    }
    
}