using HappyHeadlines.ArticleService.Entities;
using HappyHeadlines.ArticleService.DTOs;
using HappyHeadlines.ArticleService.Infrastructure;
using HappyHeadlines.ArticleService.Interfaces;
using Microsoft.AspNetCore.Mvc;

namespace HappyHeadlines.ArticleService.Controllers;

[ApiController]
[Route("api/[controller]")]
public class ArticlesController : ControllerBase
{
    private readonly IArticleRepository _articleRepository;
    private readonly ILogger<ArticlesController> _logger;
    const int pageSize = 30;

    public ArticlesController(IArticleRepository articleRepository, ILogger<ArticlesController> logger)
    {
        _articleRepository = articleRepository;
        _logger = logger;
    }

    [HttpPost("CreateArticle")]
    public async Task<IActionResult> Create(CreateArticleRequest request)
    {
        Article newArticle = new Article
        {
            Title = request.Title,
            Content = request.Content,
            Author = request.Author,
            Continent = request.Continent,
            PublishedAt = DateTime.UtcNow 
        };

        try
        {
            await _articleRepository.Create(newArticle);
        }
        catch (Exception e)
        {
            _logger.LogError("Error creating article: {Error}", e.Message);
            throw;
        }
        
        _logger.LogInformation("Creating new article by Author '{Author}' with Title '{Title}' at Articles'{Continent}' database", request.Author, request.Title, request.Continent.ToString());
        
        return CreatedAtAction(nameof(GetById), new { id = newArticle.Id }, newArticle);
    }
    
    [HttpGet("GetArticles")]
    public async Task<IActionResult> GetArticles(int page = 1, Continent continent = Continent.Global)
    {
        List<Article> articles;
        try
        {
            articles = await _articleRepository.GetAll(continent, page, pageSize);
        }
        catch (Exception ex)
        {
            _logger.LogError("Error retrieving articles: {Error}", ex.Message);
            
            return BadRequest($"Error retrieving articles: {ex.Message}");
        }

        return Ok(articles);
    }
    
    [HttpGet("GetAllRecent")]
    public async Task<IActionResult> GetAllRecent(int page = 1, Continent continent = Continent.Global)
    {
        List<Article> articles;
        try
        {
            // retrieves top pageSize articles from last 14 days
            articles = await _articleRepository.GetAllRecent( continent, page, pageSize);
        }
        catch (Exception ex)
        {
            _logger.LogError("Error retrieving articles: {Error}", ex.Message);
            return BadRequest($"Error retrieving articles: {ex.Message}");
        }

        return Ok(articles);
    }

    [HttpGet("{id:guid}")]
    public async Task<IActionResult> GetById(Guid id, Continent continent)
    {
        Article? article;
        try
        {
            article = await _articleRepository.GetById(id, continent);
        }
        catch (Exception ex)
        {
            _logger.LogError("Error fetching article: {Error}", ex.Message);
            return BadRequest($"Error fetching article: {ex.Message}");
        }
        
        return Ok(article);
    }

    [HttpPut("{id}")]
    public async Task<IActionResult> Update(Guid id, UpdateArticleRequest request)
    {
        Article? updatedArticle = new Article
        {
            Title = request.Title,
            Content = request.Content,
            Author = request.Author,
        };
        try
        {
            updatedArticle = await _articleRepository.Update(id, updatedArticle);
        }
        catch (Exception ex)
        {
            _logger.LogError("Error updating article: {Error}", ex.Message);
            return BadRequest($"Error updating article: {ex.Message}");
        }
        
        return Ok(updatedArticle);
    }

    [HttpDelete("{id}")]
    public async Task<IActionResult> Delete(Guid id, Continent continent)
    {
        try
        {
            bool deleted = await _articleRepository.Delete(id, continent);
            if (!deleted)
            {
                return NotFound("Article not found");
            }
        }
        catch (Exception ex)
        {
            _logger.LogError("Error deleting article: {Error}", ex.Message);
            return BadRequest($"Error deleting article: {ex.Message}");
        }

        return Ok();
    }
}