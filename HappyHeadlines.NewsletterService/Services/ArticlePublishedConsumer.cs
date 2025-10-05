using RabbitMQ.Client;
using RabbitMQ.Client.Events;
using HappyHeadlines.NewsletterService.Data;
using HappyHeadlines.NewsletterService.Interfaces;
using HappyHeadlines.NewsletterService.Models;
using Microsoft.EntityFrameworkCore;
using System.Text;
using System.Text.Json;
using HappyHeadlines.NewsletterService.DTOs;

namespace HappyHeadlines.NewsletterService.Services;

public class ArticlePublishedConsumer : IArticleConsumer
{
    private readonly IServiceScopeFactory _scopeFactory;
    private readonly IConfiguration _configuration;
    private readonly ILogger<ArticlePublishedConsumer> _logger;
    
    private IConnection? _connection;
    private IChannel? _channel; 
    private bool _isConsuming;
    private const int MaxRetryCount = 3;

    public ArticlePublishedConsumer(
        IConfiguration configuration, 
        ILogger<ArticlePublishedConsumer> logger, 
        IServiceScopeFactory scopeFactory)
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

        string hostName = _configuration["Rabbit:Host"] ?? _configuration["RABBITMQ_HOSTNAME"] ?? "message-broker";
        string userName = _configuration["Rabbit:User"] ?? _configuration["RABBITMQ_USERNAME"] ?? "guest";
        string password = _configuration["Rabbit:Pass"] ?? _configuration["RABBITMQ_PASSWORD"] ?? "guest";
        string queueName = _configuration["Rabbit:QueueName"] ?? "ArticleQueue";
        ushort prefetchCount = ushort.Parse(_configuration["Rabbit:PrefetchCount"] ?? "16");
        
        var factory = new ConnectionFactory()
        {
            HostName = hostName,
            UserName = userName,
            Password = password,
            AutomaticRecoveryEnabled = true, 
            NetworkRecoveryInterval = TimeSpan.FromSeconds(10)
        };

        try
        {
            _logger.LogInformation("Connecting to RabbitMQ at {HostName}...", hostName);

            _connection = await factory.CreateConnectionAsync(cancellationToken);
            _channel = await _connection.CreateChannelAsync(cancellationToken: cancellationToken); 
            
            await _channel.QueueDeclareAsync(
                queue: queueName, 
                durable: true, 
                exclusive: false, 
                autoDelete: false, 
                arguments: null,
                cancellationToken: cancellationToken);
            
            await _channel.BasicQosAsync(prefetchSize: 0, prefetchCount: prefetchCount, global: false, cancellationToken);

            var consumer = new AsyncEventingBasicConsumer(_channel);
            
            consumer.ReceivedAsync += async (sender, eventArgs) => await OnMessageReceivedAsync(eventArgs, cancellationToken);

            await _channel.BasicConsumeAsync(queue: queueName, autoAck: false, consumer: consumer, cancellationToken: cancellationToken);

            _isConsuming = true;
            _logger.LogInformation("Started consuming messages from queue '{QueueName}'", queueName);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Failed to set up RabbitMQ consumer at {HostName}", hostName);
            throw;
        }
    }

    private Task OnMessageReceivedAsync(BasicDeliverEventArgs eventArgs, CancellationToken cancellationToken)
    {
        var messageBody = eventArgs.Body.ToArray();
        var message = Encoding.UTF8.GetString(messageBody);
        
        return Task.Run(async () =>
        {
            try
            {
                await ProcessMessageAsync(message, cancellationToken);

                await _channel!.BasicAckAsync(deliveryTag: eventArgs.DeliveryTag, multiple: false);
                _logger.LogInformation("Message processed successfully (DeliveryTag: {DeliveryTag})", eventArgs.DeliveryTag);
            }
            catch (JsonException ex)
            {
                _logger.LogError(ex, "Invalid message format (DeliveryTag: {DeliveryTag}). Rejecting message.", eventArgs.DeliveryTag);
                await RejectMessageAsync(eventArgs.DeliveryTag, requeue: false);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error processing message (DeliveryTag: {DeliveryTag})", eventArgs.DeliveryTag);
                
                var retryCount = GetRetryCount(eventArgs.BasicProperties);
                
                if (retryCount >= MaxRetryCount)
                {
                    _logger.LogWarning("Max retries reached for message. Rejecting (DeliveryTag: {DeliveryTag})", eventArgs.DeliveryTag);
                    await RejectMessageAsync(eventArgs.DeliveryTag, requeue: false);
                }
                else
                {
                    _logger.LogWarning("Requeuing message (Retry {RetryCount}/{MaxRetryCount})", retryCount + 1, MaxRetryCount);
                    await RejectMessageAsync(eventArgs.DeliveryTag, requeue: true);
                }
            }
        }, cancellationToken);
    }

    private async Task RejectMessageAsync(ulong deliveryTag, bool requeue)
    {
        try
        {
            await _channel!.BasicNackAsync(deliveryTag: deliveryTag, multiple: false, requeue: requeue);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Failed to reject message (DeliveryTag: {DeliveryTag})", deliveryTag);
        }
    }

    private int GetRetryCount(IReadOnlyBasicProperties? properties)
    {
        if (properties?.Headers != null && 
            properties.Headers.TryGetValue("x-retry-count", out var retryObj))
        {
            if (retryObj is long longCount) return (int)longCount;
            if (retryObj is byte[] byteArr && int.TryParse(Encoding.UTF8.GetString(byteArr), out var intCount)) return intCount;
        }
        return 0;
    }
    
    private async Task ProcessMessageAsync(string message, CancellationToken cancellationToken)
    {
        using var scope = _scopeFactory.CreateScope();
        var serviceProvider = scope.ServiceProvider;
        
        var db = serviceProvider.GetRequiredService<AppDb>();
        var email = serviceProvider.GetRequiredService<IEmailSender>();
        var tpl = serviceProvider.GetRequiredService<ITemplateRenderer>();
        var log = serviceProvider.GetRequiredService<ILogger<ArticlePublishedConsumer>>();

        var evt = JsonSerializer.Deserialize<ArticlePublished>(message, new JsonSerializerOptions
        {
            PropertyNameCaseInsensitive = true
        });

        if (evt == null)
        {
            log.LogError("Failed to deserialize message. Message: {Message}", message);
            throw new JsonException("Failed to deserialize message into ArticlePublished.");
        }

        if (evt.ArticleId == Guid.Empty || string.IsNullOrWhiteSpace(evt.Title))
        {
            log.LogError("Invalid ArticlePublished event: missing ArticleId or Title");
            throw new JsonException("Invalid ArticlePublished event: missing required fields");
        }

        var subs = await db.Subscribers
            .Where(s => s.Confirmed && s.WantsImmediate && 
                   (s.Continent == "Global" || s.Continent == evt.Continent))
            .ToListAsync(cancellationToken);

        await using var transaction = await db.Database.BeginTransactionAsync(cancellationToken);
        
        try
        {
            var deliveriesToAdd = new List<Delivery>();
            var emailErrors = new List<string>();

            var templateModel = new Templates.TemplateModels.ImmediateModel
            {
                Title = evt.Title,
                Summary = evt.Summary,
                Url = evt.Url,
                Continent = evt.Continent,
                PublishedAtUtc = evt.PublishedAtUtc
            };

            foreach (var s in subs)
            {
                var exists = await db.Deliveries.AnyAsync(d =>
                    d.Email == s.Email && d.ArticleId == evt.ArticleId && d.Type == "Immediate",
                    cancellationToken);
                
                if (exists)
                {
                    log.LogDebug("Skipping duplicate delivery for {Email}", s.Email);
                    continue;
                }

                try
                {
                    var html = await tpl.RenderAsync("Immediate.cshtml", templateModel);
                    await email.SendAsync(s.Email, $"Breaking: {evt.Title}", html, cancellationToken);

                    deliveriesToAdd.Add(new Delivery
                    {
                        Email = s.Email,
                        ArticleId = evt.ArticleId,
                        Type = "Immediate",
                        SentAt = DateTimeOffset.UtcNow,
                        Status = "sent"
                    });
                }
                catch (Exception emailEx)
                {
                    log.LogError(emailEx, "Failed to send email to {Email}", s.Email);
                    emailErrors.Add(s.Email);
                    
                    deliveriesToAdd.Add(new Delivery
                    {
                        Email = s.Email,
                        ArticleId = evt.ArticleId,
                        Type = "Immediate",
                        SentAt = DateTimeOffset.UtcNow,
                        Status = "failed"
                    });
                }
            }

            if (deliveriesToAdd.Any())
            {
                db.Deliveries.AddRange(deliveriesToAdd);
                await db.SaveChangesAsync(cancellationToken);
            }

            await transaction.CommitAsync(cancellationToken);

            var successCount = deliveriesToAdd.Count(d => d.Status == "sent");
            log.LogInformation(
                "Immediate newsletter processing complete for Article {ArticleId}: {SuccessCount} sent, {FailCount} failed",
                evt.ArticleId, successCount, emailErrors.Count);
            
            if (emailErrors.Any())
            {
                log.LogWarning("Failed to send to: {FailedEmails}", string.Join(", ", emailErrors));
            }
        }
        catch
        {
            await transaction.RollbackAsync(cancellationToken);
            throw;
        }
    }
    
    public async Task StopConsumingAsync()
    {
        if (!_isConsuming) return;

        _logger.LogInformation("Stopping consumer...");

        _isConsuming = false;

        if (_channel != null)
        {
            await _channel.CloseAsync();
        }

        if (_connection != null)
        {
            await _connection.CloseAsync();
        }

        _logger.LogInformation("Consumer stopped successfully");
    }

    public void Dispose()
    {
        GC.SuppressFinalize(this); 
        
        _isConsuming = false;
        
        _channel?.Dispose();
        _connection?.Dispose();
    }
}