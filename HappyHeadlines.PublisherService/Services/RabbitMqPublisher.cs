using HappyHeadlines.PublisherService.Interfaces;
using RabbitMQ.Client;
using System.Text;

namespace HappyHeadlines.PublisherService.Services;

public class RabbitMqPublisher : IArticlePublisher
{
    private readonly IConfiguration _configuration;

    public RabbitMqPublisher(IConfiguration configuration)
    {
        _configuration = configuration;
    }

    public void PublishArticle(string message)
    {
        PublishArticleAsync(message).GetAwaiter().GetResult();
    }

    private async Task PublishArticleAsync(string message)
    {
        var hostName = _configuration["RABBITMQ_HOSTNAME"] ?? "localhost";
        var port = int.Parse(_configuration["RABBITMQ_PORT"] ?? "5672");
        var userName = _configuration["RABBITMQ_USERNAME"] ?? "guest";
        var password = _configuration["RABBITMQ_PASSWORD"] ?? "guest";
        var queueName = _configuration["RABBITMQ_QUEUE_NAME"] ?? "ArticleQueue";
        var virtualHost = _configuration["RABBITMQ_VIRTUAL_HOST"] ?? "/";

        ConnectionFactory factory = new ConnectionFactory()
        {
            HostName = hostName,
            Port = port,
            UserName = userName,
            Password = password,
            VirtualHost = virtualHost
        };

        await using var connection = await factory.CreateConnectionAsync();
        await using var channel = await connection.CreateChannelAsync();

        await channel.QueueDeclareAsync(
            queue: queueName,
            durable: true,
            exclusive: false,
            autoDelete: false,
            arguments: null
        );

        var body = Encoding.UTF8.GetBytes(message);
        BasicProperties properties = new BasicProperties
        {
            Persistent = true
        };

        await channel.BasicPublishAsync(
            exchange: string.Empty,
            routingKey: queueName,
            mandatory: false,
            basicProperties: properties,
            body: body
        );
    }
}