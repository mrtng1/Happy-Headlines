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

    public ArticlePublishedController(IArticlePublisher articlePublisher)
    {
        _articlePublisher = articlePublisher;
    }

    [HttpPost("publish")]
    public IActionResult PublishArticle([FromBody] PublishArticleRequest request)
    {
        string message = JsonSerializer.Serialize(request);
        
        // Publish to queue
        _articlePublisher.PublishArticle(message);

        return Ok(new { Message = "Article published to queue successfully." });
    }
}