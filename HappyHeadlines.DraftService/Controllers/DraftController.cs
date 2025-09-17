using HappyHeadlines.Core.DTOs;
using HappyHeadlines.Core.Entities;
using Microsoft.AspNetCore.Mvc;
using HappyHeadlines.Core.Interfaces;

namespace HappyHeadlines.DraftService.Controllers;

[ApiController]
[Route("api/[controller]")]
public class DraftController : ControllerBase
{
    private readonly IRepository<Article> _articleRepository;
    const int pageSize = 30;

    public DraftController(IRepository<Article> articleRepository)
    {
        _articleRepository = articleRepository;
    }

    [HttpPost]
    public async Task<IActionResult> Create(CreateArticleRequest request)
    {
        throw new NotImplementedException();
    }
    
    [HttpGet("GetDrafts")]
    public async Task<IActionResult> GetArticles(int page, Continent continent)
    {
        throw new NotImplementedException();
    }

    [HttpGet("{id}")]
    public async Task<IActionResult> GetById(Guid id)
    {
        throw new NotImplementedException();
    }

    [HttpPut("{id}")]
    public async Task<IActionResult> Update(Guid id, UpdateArticleRequest request)
    {
        throw new NotImplementedException();
    }

    [HttpDelete("{id}")]
    public async Task<IActionResult> Delete(Guid id)
    {
        throw new NotImplementedException();
    }
}
