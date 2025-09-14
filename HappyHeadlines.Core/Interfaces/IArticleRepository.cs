using HappyHeadlines.Core.DTOs;
using HappyHeadlines.Core.Entities;

namespace HappyHeadlines.Core.Interfaces;

public interface IRepository<T>
{
    Task<IEnumerable<T>> GetAll();
    Task<T?> GetById(Guid id);
    Task<T> Create(CreateArticleRequest request);
    Task<T?> Update(Guid id, UpdateArticleRequest request);
    Task<bool> Delete(Guid id, Continent continent);
}