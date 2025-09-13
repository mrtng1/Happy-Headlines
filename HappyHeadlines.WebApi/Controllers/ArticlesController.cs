using HappyHeadlines.Core.DTOs;
using HappyHeadlines.Core.Entities;
using Microsoft.AspNetCore.Mvc;
using HappyHeadlines.Core.Interfaces;
namespace HappyHeadlines.WebApi.Controllers;

[ApiController]
[Route("api/[controller]")]
public class ArticlesController : ControllerBase
{
    private readonly IArticleRepository _articleRepository;

    public ArticlesController(IArticleRepository articleRepository)
    {
        _articleRepository = articleRepository;
    }

    [HttpPost]
    public async Task<IActionResult> Create(CreateArticleRequest request)
    {
        Article createdArticle;
        try
        {
            createdArticle = await  _articleRepository.CreateArticle(request);
        }
        catch (Exception ex)
        {
            return BadRequest($"Error creating article: {ex.Message}");
        }

        return CreatedAtAction(nameof(GetById), new { id = createdArticle.Id });
    }

    [HttpGet("{id}")]
    public async Task<IActionResult> GetById(Guid id)
    {
        Article? article;
        try
        {
            article = await _articleRepository.GetArticleById(id);
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
        Article? article;
        try
        {
            article = await _articleRepository.UpdateArticle(id, request);
        }
        catch (Exception ex)
        {
            return BadRequest($"Error updating article: {ex.Message}");
        }
        
        return Ok(article);
    }

    [HttpDelete("{id}")]
    public async Task<IActionResult> Delete(Guid id)
    {
        try
        {
            bool deleted = await _articleRepository.DeleteArticle(id);
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
