using HappyHeadlines.Core.Entities;
using HappyHeadlines.Core.Interfaces;
using Microsoft.EntityFrameworkCore;

namespace HappyHeadlines.Infrastructure.Repositories;

public class DraftRepository : IRepository<Draft>
{
    private readonly DbContextFactory _contextFactory;

    public DraftRepository(DbContextFactory contextFactory)
    {
        _contextFactory = contextFactory;
    }
    
    public async Task<List<Draft>> GetAll(Continent continent, int pageNumber = 1, int pageSize = 10)
    {
        await using var context = _contextFactory.Create(continent);
        return await context.Drafts
            .Skip((pageNumber - 1) * pageSize)
            .Take(pageSize)
            .ToListAsync();
    }
    
    public async Task<Draft> Create(Draft newDraft)
    {
        await using var context = _contextFactory.Create(newDraft.Continent);

        try
        {
            newDraft = context.Drafts.Add(newDraft).Entity;
            
            await context.SaveChangesAsync();
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
        await using var context = _contextFactory.Create(continent);

        Draft? draft = await context.Drafts.FindAsync(id);

        if (draft == null)
            return false;

        context.Drafts.Remove(draft);
        await context.SaveChangesAsync();
        return true;
    }

    public async Task<Draft?> GetById(Guid id, Continent continent)
    {
        await using var context = _contextFactory.Create(continent);
        Draft? draft = await context.Drafts.FindAsync(id);
        if (draft != null)
        {
            return draft;
        }
        
        return null;
    }

    public async Task<Draft?> Update(Guid id, Draft updatedDraft)
    {
        await using var context = _contextFactory.Create(updatedDraft.Continent);

        Draft? existingDraft = await context.Drafts.FindAsync(id);

        if (existingDraft == null)
        {
            return null;
        }

        existingDraft.Title = updatedDraft.Title;
        existingDraft.Content = updatedDraft.Content;
        existingDraft.AuthorId = updatedDraft.AuthorId;
            
        await context.SaveChangesAsync();
        return existingDraft;
    }
    
}