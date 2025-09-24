namespace HappyHeadlines.CommentService.DTOs;

using System.ComponentModel.DataAnnotations;

public class CreateCommentRequest
{
    [Required]
    public Guid ArticleId { get; set; }

    [Required]
    public Guid AuthorId { get; set; }

    [Required]
    [StringLength(2000, MinimumLength = 1)]
    public string Content { get; set; }
}