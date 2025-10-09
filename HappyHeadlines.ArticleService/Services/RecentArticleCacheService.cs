using HappyHeadlines.ArticleService.Entities;
using System.Text.Json;
using HappyHeadlines.ArticleService.Infrastructure;
using StackExchange.Redis;

namespace HappyHeadlines.ArticleService.Services;

public class RecentArticleCacheService : IHostedService, IDisposable // - background service
{
    private readonly IServiceProvider _services;
    private readonly ILogger<RecentArticleCacheService> _logger;
    private Timer? _timer;
    private const string RecentArticlesCacheKey = "articles:recent_14_days";

    public RecentArticleCacheService(IServiceProvider services, ILogger<RecentArticleCacheService> logger)
    {
        _services = services;
        _logger = logger;
    }

    public Task StartAsync(CancellationToken cancellationToken)
    {
        _logger.LogInformation("Starting the scheduled service to cache recent articles.");

        //_timer = new Timer(DoWork, null, TimeSpan.Zero, TimeSpan.FromHours(1));
        _timer = new Timer(DoWork, null, TimeSpan.Zero, TimeSpan.FromMinutes(1));
        return Task.CompletedTask;
    }

    // This is the actual job that gets run by the timer.
    private void DoWork(object? state)
    {
        _logger.LogInformation("Job started: Updating the cache with articles from the last 14 days.");

        using var scope = _services.CreateScope();
        ArticleRepository repository = scope.ServiceProvider.GetRequiredService<ArticleRepository>();
        var redis = scope.ServiceProvider.GetRequiredService<IConnectionMultiplexer>();

        try
        {
            // Fetch all articles from the last 14 days from the database
            List<Article> recentArticles = repository.GetAllRecent(Continent.Global).GetAwaiter().GetResult();

            if (!recentArticles.Any())
            {
                _logger.LogWarning("No recent articles found in the database to cache.");
                return;
            }

            var cacheDb = redis.GetDatabase();
            var serializedArticles = JsonSerializer.Serialize(recentArticles);

            // 2. Save the entire list to Redis under our special key.
            cacheDb.StringSet(RecentArticlesCacheKey, serializedArticles, expiry: TimeSpan.FromHours(2));
            
            _logger.LogInformation("Job finished: Successfully cached {Count} articles to key '{Key}'",recentArticles.Count, RecentArticlesCacheKey);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "The background job to cache recent articles failed.");
        }
    }

    public Task StopAsync(CancellationToken cancellationToken)
    {
        _logger.LogInformation("Stopping the recent article cache service.");
        _timer?.Change(Timeout.Infinite, 0);
        return Task.CompletedTask;
    }

    public void Dispose() => _timer?.Dispose();
}