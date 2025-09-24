using HappyHeadlines.Core.DTOs;
using HappyHeadlines.Core.Entities;
using Microsoft.AspNetCore.Mvc;
using HappyHeadlines.DraftService.Interfaces;

namespace HappyHeadlines.DraftService.Controllers;

[ApiController]
[Route("api/[controller]")]
public class DraftController : ControllerBase
{
    private readonly IDraftRepository _draftRepository;
    private readonly ILogger<DraftController> _logger;
    private const int PageSize = 30;

    public DraftController(IDraftRepository draftRepository, ILogger<DraftController> logger)
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

        try
        {
            await _draftRepository.Create(newDraft);
        }
        catch (Exception e)
        {
            MonitorService.MonitorService.Log.Error("Error creating draft: {Error}", e.Message);
            return StatusCode(500, "An error occurred while creating the draft.");
        }

        _logger.LogInformation("Successfully created draft with ID {DraftId}", newDraft.Id);

        return CreatedAtAction(nameof(GetById), new { id = newDraft.Id }, newDraft);
    }
    
    [HttpGet]
    public async Task<IActionResult> GetDrafts([FromQuery] int page = 1, [FromQuery] Continent continent = Continent.Global)
    {
        _logger.LogInformation("Fetching drafts for page {Page} and continent {Continent}", page, continent);

        List<Draft> drafts = new List<Draft>();
        
        try
        {
            drafts = await _draftRepository.GetAll(continent, page, PageSize);
        }
        catch (Exception ex)
        {
            MonitorService.MonitorService.Log.Error("Error retrieving drafts: {Error}", ex.Message);
            return StatusCode(500, "An error occurred while retrieving drafts.");
        }
        
        return Ok(drafts);
    }

    [HttpGet("{id:guid}")]
    public async Task<IActionResult> GetById(Guid id, Continent continent)
    {
        _logger.LogInformation("Fetching draft with ID {DraftId}", id);

        Draft? draft;
        try
        {
            draft = await _draftRepository.GetById(id, continent);
        }
        catch (Exception ex)
        {
            MonitorService.MonitorService.Log.Error("Error retrieving draft: {Error}", ex.Message);
            return StatusCode(500, "An error occurred while retrieving drafts.");
        }

        if (draft == null)
        {
            return NotFound();
        }

        return Ok(draft);
    }

    [HttpPut("{id:guid}")]
    public async Task<IActionResult> Update(Guid id, UpdateDraftRequest request)
    {

        Draft? updatedDraft = new Draft()
        {
            Title = request.Title,
            Content = request.Content,
        };

        try
        {
            await _draftRepository.Update(id, updatedDraft);
        }
        catch (Exception e)
        {
            MonitorService.MonitorService.Log.Error("Error updating draft: {Error}", e.Message);
            throw;
        }
        
        return NoContent();
    }

    [HttpDelete("{id:guid}")]
    public async Task<IActionResult> Delete(Guid id, Continent continent)
    {

        try
        {
            await _draftRepository.Delete(id, continent);
        }
        catch (Exception e)
        {
            MonitorService.MonitorService.Log.Error("Error deleting draft: {Error}", e.Message);
            throw;
        }

        return NoContent();
    }
}