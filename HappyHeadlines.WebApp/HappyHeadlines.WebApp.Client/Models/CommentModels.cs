using System.ComponentModel.DataAnnotations;

namespace HappyHeadlines.WebApp.Client.Models;

public class CommentItem
{
    public Guid Id { get; set; }
    public Guid ArticleId { get; set; }
    public string Author { get; set; } = "";
    public string Content { get; set; } = "";
    public DateTime CreatedAt { get; set; }
}

public class CreateCommentRequest
{
    [Required] public Guid ArticleId { get; set; }
    [Required, StringLength(80)] public string Author { get; set; } = "";
    [Required, MinLength(3)] public string Content { get; set; } = "";
}
