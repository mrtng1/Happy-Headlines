namespace HappyHeadlines.NewsletterService.Interfaces;

public interface IArticleConsumer : IDisposable
{
    Task StartConsumingAsync(CancellationToken cancellationToken = default);
    Task StopConsumingAsync();
}