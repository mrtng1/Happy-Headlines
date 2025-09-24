using HappyHeadlines.Core.Entities;
using HappyHeadlines.DraftService.Interfaces;
using Microsoft.EntityFrameworkCore;

namespace HappyHeadlines.Infrastructure.Repositories;

public class DraftRepository : IDraftRepository
{

    private readonly DraftDbContext _context;
    public DraftRepository(DraftDbContext context)
    {
        _context = context;
    }
    
    public async Task<List<Draft>> GetAll(Continent continent, int pageNumber = 1, int pageSize = 10)
    {
        return await _context.Drafts
            .Skip((pageNumber - 1) * pageSize)
            .Take(pageSize)
            .ToListAsync();
    }
    
    public async Task<Draft> Create(Draft newDraft)
    {
        try
        {
            newDraft = _context.Drafts.Add(newDraft).Entity;
            
            await _context.SaveChangesAsync();
        }
        catch (Exception e)
        {
            Console.WriteLine(e);
            throw;
        }
    
        return newDraft;
    }


    public async Task<bool> Delete(Guid id, Continent continent)
    {
        Draft? draft = await _context.Drafts.FindAsync(id);

        if (draft == null)
            return false;

        _context.Drafts.Remove(draft);
        await _context.SaveChangesAsync();
        return true;
    }

    public async Task<Draft?> GetById(Guid id, Continent continent)
    {
        Draft? draft = await _context.Drafts.FindAsync(id);
        if (draft != null)
        {
            return draft;
        }
        
        return null;
    }

    public async Task<Draft?> Update(Guid id, Draft updatedDraft)
    {
        Draft? existingDraft = await _context.Drafts.FindAsync(id);

        if (existingDraft == null)
        {
            return null;
        }

        existingDraft.Title = updatedDraft.Title;
        existingDraft.Content = updatedDraft.Content;
        existingDraft.AuthorId = updatedDraft.AuthorId;
            
        await _context.SaveChangesAsync();
        return existingDraft;
    }
    
}