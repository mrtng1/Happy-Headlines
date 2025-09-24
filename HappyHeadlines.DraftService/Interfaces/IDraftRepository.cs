using HappyHeadlines.Core.Entities;

namespace HappyHeadlines.DraftService.Interfaces;

public interface IDraftRepository
{
    
    Task<List<Draft>> GetAll(Continent continent, int pageNumber = 1, int pageSize = 10);
    Task<Draft> Create(Draft newDraft);
    Task<bool> Delete(Guid id, Continent continent);
    Task<Draft?> GetById(Guid id, Continent continent);
    Task<Draft?> Update(Guid id, Draft updatedDraft);
}