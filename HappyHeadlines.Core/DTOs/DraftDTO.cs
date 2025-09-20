using HappyHeadlines.Core.Entities;

namespace HappyHeadlines.Core.DTOs;

public record CreateDraftRequest(Guid AuthorId, string Title, string Content, Continent Continent);
public record UpdateDraftRequest(string Title, string Content);