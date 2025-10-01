using RabbitMQ.Client;
using RabbitMQ.Client.Events;
using System.Text;
using System.Text.Json;
using HappyHeadlines.ArticleService.DTOs;
using HappyHeadlines.ArticleService.Entities;
using HappyHeadlines.ArticleService.Interfaces;

namespace HappyHeadlines.ArticleService.Services;

public class ArticleConsumer : IArticleConsumer, IDisposable
{
    private readonly IServiceScopeFactory _scopeFactory; 
    private readonly IConfiguration _configuration;
    private readonly ILogger<ArticleConsumer> _logger;
    private IConnection? _connection;
    private IChannel? _channel;
    private bool _isConsuming;

    public ArticleConsumer(IConfiguration configuration, ILogger<ArticleConsumer> logger, IServiceScopeFactory scopeFactory)
    {
        _configuration = configuration;
        _logger = logger;
        _scopeFactory = scopeFactory;
    }

    public async Task StartConsumingAsync(CancellationToken cancellationToken = default)
    {
        if (_isConsuming)
        {
            _logger.LogWarning("Consumer is already running");
            return;
        }

        var hostName = _configuration["RABBITMQ_HOSTNAME"] ?? "localhost";
        var port = int.Parse(_configuration["RABBITMQ_PORT"] ?? "5672");
        var userName = _configuration["RABBITMQ_USERNAME"] ?? "guest";
        var password = _configuration["RABBITMQ_PASSWORD"] ?? "guest";
        var queueName = _configuration["RABBITMQ_QUEUE_NAME"] ?? "ArticleQueue";
        var virtualHost = _configuration["RABBITMQ_VIRTUAL_HOST"] ?? "/";
        var prefetchCount = ushort.Parse(_configuration["RABBITMQ_PREFETCH_COUNT"] ?? "1");

        // Setup the connection factory
        ConnectionFactory factory = new ConnectionFactory()
        {
            HostName = hostName,
            Port = port,
            UserName = userName,
            Password = password,
            VirtualHost = virtualHost,
            RequestedConnectionTimeout = TimeSpan.FromSeconds(30),
            SocketReadTimeout = TimeSpan.FromSeconds(30),
            SocketWriteTimeout = TimeSpan.FromSeconds(30),
        };

        try
        {
            _logger.LogInformation("Connecting to RabbitMQ at {HostName}:{Port}...", hostName, port);

            _connection = await factory.CreateConnectionAsync(cancellationToken);
            _channel = await _connection.CreateChannelAsync(cancellationToken: cancellationToken);

            await _channel.BasicQosAsync(prefetchSize: 0, prefetchCount: prefetchCount, global: false, cancellationToken);

            await _channel.QueueDeclareAsync(
                queue: queueName,
                durable: true,
                exclusive: false,
                autoDelete: false,
                arguments: null,
                cancellationToken: cancellationToken
            );

            // Create consumer
            var consumer = new AsyncEventingBasicConsumer(_channel);
            consumer.ReceivedAsync += async (sender, eventArgs) =>
            {
                await OnMessageReceivedAsync(eventArgs, cancellationToken);
            };

            // Start consuming
            await _channel.BasicConsumeAsync(
                queue: queueName,
                autoAck: false,
                consumer: consumer,
                cancellationToken: cancellationToken
            );

            _isConsuming = true;
            _logger.LogInformation("Started consuming messages from queue '{QueueName}'", queueName);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Failed to connect to RabbitMQ at {HostName}:{Port}", hostName, port);
            throw new InvalidOperationException(
                $"Failed to connect to RabbitMQ at {hostName}:{port}. " +
                $"Please ensure RabbitMQ is running. Error: {ex.Message}", ex);
        }
    }

    private async Task OnMessageReceivedAsync(BasicDeliverEventArgs eventArgs, CancellationToken cancellationToken)
    {
        var message = Encoding.UTF8.GetString(eventArgs.Body.ToArray());
        
        try
        {
            _logger.LogInformation("Received message: {Message}", message);

            await ProcessMessageAsync(message, cancellationToken);

            // Acknowledge the message (remove from queue)
            if (_channel != null)
            {
                await _channel.BasicAckAsync(deliveryTag: eventArgs.DeliveryTag, multiple: false, cancellationToken);
                _logger.LogInformation("Message processed and acknowledged successfully");
            }
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error processing message: {Message}", message);
            
            if (_channel != null)
            {
                await _channel.BasicNackAsync(
                    deliveryTag: eventArgs.DeliveryTag,
                    multiple: false,
                    requeue: true,
                    cancellationToken);
            }
        }
    }

    private async Task ProcessMessageAsync(string message, CancellationToken cancellationToken)
    {
        _logger.LogInformation("Processing article message: {Message}", message);

        try
        {
            // Deserialize JSON message into CreateArticleRequest DTO
            var createArticleRequest = JsonSerializer.Deserialize<CreateArticleRequest>(message, new JsonSerializerOptions
            {
                PropertyNameCaseInsensitive = true
            });

            if (createArticleRequest == null)
            {
                _logger.LogError("Failed to deserialize message into CreateArticleRequest. Message: {Message}", message);
                return;
            }

            using (var scope = _scopeFactory.CreateScope())
            {
                var articleRepository = scope.ServiceProvider.GetRequiredService<IArticleRepository>();

                Article newArticle = new Article
                {
                    Title = createArticleRequest.Title,
                    Content = createArticleRequest.Content,
                    Author = createArticleRequest.Author,
                    Continent = createArticleRequest.Continent,
                    PublishedAt = DateTime.UtcNow 
                };

                Article createdArticle = await articleRepository.Create(newArticle);
            
                _logger.LogInformation("Article created successfully with ID: {ArticleId}", createdArticle.Id);
            }
        }
        catch (JsonException jEx)
        {
            _logger.LogError(jEx, "JSON Deserialization Error for message: {Message}", message);
            throw; 
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error during article processing for message: {Message}", message);
            throw;
        }
    }

    public async Task StopConsumingAsync()
    {
        if (!_isConsuming)
        {
            _logger.LogWarning("Consumer is not running");
            return;
        }

        _logger.LogInformation("Stopping consumer...");

        if (_channel != null)
        {
            await _channel.CloseAsync();
        }

        if (_connection != null)
        {
            await _connection.CloseAsync();
        }

        _isConsuming = false;
        _logger.LogInformation("Consumer stopped successfully");
    }

    public void Dispose()
    {
        if (_isConsuming)
            StopConsumingAsync().GetAwaiter().GetResult();

        _channel?.Dispose();
        _connection?.Dispose();
    }
}
