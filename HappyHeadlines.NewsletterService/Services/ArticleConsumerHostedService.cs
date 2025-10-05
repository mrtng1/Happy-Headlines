using HappyHeadlines.NewsletterService.Interfaces;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;

namespace HappyHeadlines.NewsletterService.Services;

public class ArticleConsumerHostedService : IHostedService, IDisposable
{
    private readonly IArticleConsumer _consumer;
    private readonly ILogger<ArticleConsumerHostedService> _logger;

    public ArticleConsumerHostedService(IArticleConsumer consumer, ILogger<ArticleConsumerHostedService> logger)
    {
        _consumer = consumer;
        _logger = logger;
    }

    public Task StartAsync(CancellationToken cancellationToken)
    {
        _logger.LogInformation("Article Consumer Hosted Service starting.");
        
        // Start the consumer logic in a background task
        _ = Task.Run(() => _consumer.StartConsumingAsync(cancellationToken), cancellationToken);

        return Task.CompletedTask;
    }

    public async Task StopAsync(CancellationToken cancellationToken)
    {
        _logger.LogInformation("Article Consumer Hosted Service stopping.");
        await _consumer.StopConsumingAsync(); 
    }

    public void Dispose()
    {
        if (_consumer is IDisposable disposableConsumer)
        {
            disposableConsumer.Dispose();
        }
    }
}