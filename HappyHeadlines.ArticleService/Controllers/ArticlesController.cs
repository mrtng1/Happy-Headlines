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
    const int pageSize = 30;

    public ArticlesController(IArticleRepository articleRepository)
    {
        _articleRepository = articleRepository;
    }

    [HttpPost]
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
            MonitorService.MonitorService.Log.Error("Error creating article: {Error}", e.Message);
            throw;
        }
        
        MonitorService.MonitorService.Log.Information("Creating new article by Author '{Author}' with Title '{Title}' at Articles'{Continent}' database", request.Author, request.Title, request.Continent.ToString());
        
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
            MonitorService.MonitorService.Log.Error("Error retrieving articles: {Error}", ex.Message);
            return BadRequest($"Error retrieving articles: {ex.Message}");
        }

        return Ok(articles);
    }

    [HttpGet("{id}")]
    public async Task<IActionResult> GetById(Guid id, Continent continent)
    {
        Article? article;
        try
        {
            article = await _articleRepository.GetById(id, continent);
        }
        catch (Exception ex)
        {
            MonitorService.MonitorService.Log.Error("Error fetching article: {Error}", ex.Message);
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
            MonitorService.MonitorService.Log.Error("Error updating article: {Error}", ex.Message);
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
            MonitorService.MonitorService.Log.Error("Error deleting article: {Error}", ex.Message);
            return BadRequest($"Error deleting article: {ex.Message}");
        }

        return Ok();
    }
}
