using System.Text.Json;
using HappyHeadlines.PublisherService.DTOs;
using HappyHeadlines.PublisherService.Interfaces;
using Microsoft.AspNetCore.Mvc;

namespace HappyHeadlines.PublisherService.Controllers;

[ApiController]
[Route("api/Publisher")]
public class ArticlePublishedController : ControllerBase
{
    private readonly IArticlePublisher _articlePublisher;
    private readonly ILogger<ArticlePublishedController> _logger;

    public ArticlePublishedController(IArticlePublisher articlePublisher, ILogger<ArticlePublishedController> logger)
    {
        _articlePublisher = articlePublisher;
        _logger = logger;
    }
    
    [HttpPost("publish")]
    public IActionResult PublishArticle([FromBody] CreateArticleRequest request)
    {
        try
        {
            _logger.LogInformation("Received request to publish article with title: {Title}", request.Title);
            
            string message = JsonSerializer.Serialize(request);
        
            // Publish to queue
            _articlePublisher.PublishArticle(message);
        }
        catch (Exception ex)
        {
            // Log the error
            _logger.LogError(ex, "Error publishing article: {Message}", ex.Message);
            return BadRequest(new { Message = $"Error publishing article: {ex.Message}" });
        }

        return Ok(new { Message = "Article published to queue successfully." });
    }
}