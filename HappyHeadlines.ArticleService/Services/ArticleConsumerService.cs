namespace HappyHeadlines.ArticleService.Services;

using Interfaces;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;


public class ArticleConsumerService : IHostedService, IDisposable
{
    private readonly IArticleConsumer _consumer;
    private readonly ILogger<ArticleConsumerService> _logger;

    public ArticleConsumerService(IArticleConsumer consumer, ILogger<ArticleConsumerService> logger)
    {
        _consumer = consumer;
        _logger = logger;
    }

    public Task StartAsync(CancellationToken cancellationToken)
    {
        _logger.LogInformation("Article Consumer Hosted Service starting.");
        
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