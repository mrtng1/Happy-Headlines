using HappyHeadlines.Core.DTOs;
using HappyHeadlines.Core.Entities;

namespace HappyHeadlines.Core.Interfaces;

public interface IRepository<T>
{
    Task<List<T>> GetAll(Continent continent, int pageNumber = 1, int pageSize = 10);
    Task<T?> GetById(Guid id, Continent continent);
    Task<T> Create(T request);
    Task<T?> Update(Guid id, T updatedDraft);
    Task<bool> Delete(Guid id, Continent continent);
}