using HappyHeadlines.CommentService.DTOs;
using HappyHeadlines.CommentService.Entities;
using HappyHeadlines.CommentService.Interfaces;
using Polly.CircuitBreaker;

namespace HappyHeadlines.CommentService.Services;

public class CommentService : ICommentService
{
    private readonly ICommentRepository _commentRepository;
    private readonly IProfanityClient _profanityClient;

    public CommentService(ICommentRepository commentRepository, IProfanityClient profanityClient)
    {
        _commentRepository = commentRepository;
        _profanityClient = profanityClient;
    }

    public async Task<CommentDto> CreateComment(CreateCommentRequest request)
    {
        // call profanity service
        bool containsProfanity = false;
        try
        {
            // Call the Profanity Service
            containsProfanity = await _profanityClient.ContainsProfanityAsync(request.Content);
        }
        // Catch the exception when the circuit is open
        catch (BrokenCircuitException)
        {
            // Case of OUTAGE - circuit breaker active
            throw new Exception("Profanity Service is currently unavailable. Please try again later.");
        }
        catch (HttpRequestException ex)
        {
            //_logger.LogError(ex, "An HTTP error occurred while calling ProfanityService.");
            throw;
        }
        
        if (containsProfanity)
        {
            throw new Exception("Comment contains profanity.");
        }
        
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