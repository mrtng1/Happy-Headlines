namespace HappyHeadlines.CommentService.Entities;

public class CommentDto
{
    public Guid Id { get; set; }
    public Guid ArticleId { get; set; }
    public Guid AuthorId { get; set; }
    public string Content { get; set; }
    public DateTimeOffset CreatedAt { get; set; }
}