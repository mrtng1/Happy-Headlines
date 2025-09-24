namespace HappyHeadlines.CommentService.Entities;

using System;
using System.ComponentModel.DataAnnotations;


public class Comment
{
    [Key]
    public Guid Id { get; set; }

    [Required]
    public Guid ArticleId { get; set; }

    [Required]
    public Guid AuthorId { get; set; }

    [Required]
    [StringLength(2000, MinimumLength = 1)]
    public string Content { get; set; }

    public DateTimeOffset CreatedAt { get; set; } = DateTimeOffset.UtcNow;
}