using HappyHeadlines.CommentService.DTOs;
using HappyHeadlines.CommentService.Entities;
using HappyHeadlines.CommentService.Interfaces;

namespace HappyHeadlines.CommentService.Services;

public class CommentService : ICommentService
{
    private readonly ICommentRepository _commentRepository;

    public CommentService(ICommentRepository commentRepository)
    {
        _commentRepository = commentRepository;
    }

    public async Task<CommentDto> CreateComment(CreateCommentRequest request)
    {
        Comment comment = new Comment
        {
            ArticleId = request.ArticleId,
            AuthorId = request.AuthorId,
            Content = request.Content
        };

        await _commentRepository.CreateComment(comment);

        return new CommentDto
        {
            Id = comment.Id,
            ArticleId = comment.ArticleId,
            AuthorId = comment.AuthorId,
            Content = comment.Content,
            CreatedAt = comment.CreatedAt
        };
    }

    public async Task<List<CommentDto>> GetCommentsByArticleId(Guid articleId, int page, int pageSize)
    {
        List<Comment> comments = await _commentRepository.GetCommentsByArticleId(articleId, page, pageSize);

        return comments.Select(comment => new CommentDto
        {
            Id = comment.Id,
            ArticleId = comment.ArticleId,
            AuthorId = comment.AuthorId,
            Content = comment.Content,
            CreatedAt = comment.CreatedAt
        }).ToList();
    }
        
    public async Task<bool> DeleteComment(Guid id)
    {
        return await _commentRepository.DeleteComment(id);
    }
}