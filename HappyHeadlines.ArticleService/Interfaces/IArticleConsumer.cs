namespace HappyHeadlines.ArticleService.Interfaces;

public interface IArticleConsumer
{
    Task StartConsumingAsync(CancellationToken cancellationToken = default);
    Task StopConsumingAsync();
}