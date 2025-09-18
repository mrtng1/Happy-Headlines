using HappyHeadlines.Core.DTOs;
using HappyHeadlines.Core.Entities;
using Microsoft.AspNetCore.Mvc;
using HappyHeadlines.Core.Interfaces;

namespace HappyHeadlines.DraftService.Controllers;

[ApiController]
[Route("api/[controller]")]
public class DraftController : ControllerBase
{
    private readonly IRepository<Draft> _draftRepository;
    private readonly ILogger<DraftController> _logger;
    private const int PageSize = 30;

    public DraftController(IRepository<Draft> draftRepository, ILogger<DraftController> logger)
    {
        _draftRepository = draftRepository;
        _logger = logger;
    }

    [HttpPost]
    public async Task<IActionResult> Create(CreateDraftRequest request)
    {
        _logger.LogInformation("Attempting to create a new draft titled '{DraftTitle}'", request.Title);

        var newDraft = new Draft
        {
            Title = request.Title,
            Content = request.Content,
            AuthorId = request.AuthorId,
            Continent = request.Continent
        };

        await _draftRepository.Create(newDraft);

        _logger.LogInformation("Successfully created draft with ID {DraftId}", newDraft.Id);

        return CreatedAtAction(nameof(GetById), new { id = newDraft.Id }, newDraft);
    }
    
    [HttpGet]
    public async Task<IActionResult> GetDrafts([FromQuery] int page = 1, [FromQuery] Continent continent = Continent.Global)
    {
        _logger.LogInformation("Fetching drafts for page {Page} and continent {Continent}", page, continent);

        var drafts = await _draftRepository.GetAll(continent, page, PageSize);
        
        _logger.LogInformation("Found {DraftCount} drafts.", drafts.Count());

        return Ok(drafts);
    }

    [HttpGet("{id:guid}")]
    public async Task<IActionResult> GetById(Guid id)
    {
        _logger.LogInformation("Fetching draft with ID {DraftId}", id);
        Draft draft = await _draftRepository.GetById(id);

        if (draft == null)
        {
            _logger.LogWarning("Draft with ID {DraftId} not found.", id);
            return NotFound();
        }

        return Ok(draft);
    }

    [HttpPut("{id:guid}")]
    public async Task<IActionResult> Update(Guid id, UpdateDraftRequest request)
    {
        _logger.LogInformation("Attempting to update draft with ID {DraftId}", id);
        var existingDraft = await _draftRepository.GetById(id);

        if (existingDraft == null)
        {
            _logger.LogWarning("Update failed. Draft with ID {DraftId} not found.", id);
            return NotFound();
        }

        existingDraft.Title = request.Title;
        existingDraft.Content = request.Content;
        existingDraft.LastModifiedAt = DateTime.UtcNow;
        existingDraft.Version++;

        try
        {
            await _draftRepository.Update(id, existingDraft);
        }
        catch (Exception e)
        {
            _logger.LogError("Error updating draft with ID {DraftId}: {ErrorMessage}", id, e.Message);
            throw;
        }
        
        _logger.LogInformation("Successfully updated draft with ID {DraftId}", id);

        return NoContent();
    }

    [HttpDelete("{id:guid}")]
    public async Task<IActionResult> Delete(Guid id, Continent continent)
    {
        _logger.LogInformation("Attempting to delete draft with ID {DraftId}", id);

        try
        {
            await _draftRepository.Delete(id, continent);
        }
        catch (Exception e)
        {
            _logger.LogInformation("Error deleting draft with ID {DraftId}: {ErrorMessage}", id, e.Message);
            throw;
        }
        _logger.LogInformation("Successfully deleted draft with ID {DraftId}", id);

        return NoContent();
    }
}