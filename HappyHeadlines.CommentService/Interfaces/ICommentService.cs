using HappyHeadlines.CommentService.DTOs;
using HappyHeadlines.CommentService.Entities;

namespace HappyHeadlines.CommentService.Interfaces;

public interface ICommentService
{
    Task<CommentDto> CreateComment(CreateCommentRequest request);
    Task<List<CommentDto>> GetCommentsByArticleId(Guid articleId, int page, int pageSize);
    Task<bool> DeleteComment(Guid id);
}