using HappyHeadlines.Core.DTOs;
using HappyHeadlines.Core.Entities;
using Microsoft.AspNetCore.Mvc;
using HappyHeadlines.Core.Interfaces;
using HappyHeadlines.MonitorService;

namespace HappyHeadlines.ArticleService.Controllers;

[ApiController]
[Route("api/[controller]")]
public class ArticlesController : ControllerBase
{
    private readonly IRepository<Article> _articleRepository;
    const int pageSize = 30;

    public ArticlesController(IRepository<Article> articleRepository)
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
        
        await _articleRepository.Create(newArticle);
        
        MonitorService.MonitorService.Log.Information("Creating new article by Author '{Author}' with Title '{Title}' at Articles'{Continent}' database", request.Author, request.Title, request.Continent.ToString());
        
        return CreatedAtAction(nameof(GetById), new { id = newArticle.Id }, newArticle);
    }
    
    [HttpGet("GetArticles")]
    public async Task<IActionResult> GetArticles(int page, Continent continent)
    {
        List<Article> articles;
        try
        {
            articles = await _articleRepository.GetAll(continent, page, pageSize);
        }
        catch (Exception ex)
        {
            return BadRequest($"Error retrieving articles: {ex.Message}");
        }

        return Ok(articles);
    }

    [HttpGet("{id}")]
    public async Task<IActionResult> GetById(Guid id)
    {
        Article? article;
        try
        {
            article = await _articleRepository.GetById(id);
        }
        catch (Exception ex)
        {
            return BadRequest($"Error fetching article: {ex.Message}");
        }
        
        return Ok(article);
    }

    [HttpPut("{id}")]
    public async Task<IActionResult> Update(Guid id, UpdateArticleRequest request)
    {
        Article updatedArticle = new Article
        {
            Title = request.Title,
            Content = request.Content,
            Author = request.Author,
            Continent = request.Continent,
        };
        try
        {
            updatedArticle = await _articleRepository.Update(id, updatedArticle);
        }
        catch (Exception ex)
        {
            return BadRequest($"Error updating article: {ex.Message}");
        }
        
        return Ok(updatedArticle);
    }

    [HttpDelete("{id}")]
    public async Task<IActionResult> Delete(Guid id)
    {
        Continent continent = Continent.Australia; // test
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
            return BadRequest($"Error deleting article: {ex.Message}");
        }

        return Ok();
    }
}
