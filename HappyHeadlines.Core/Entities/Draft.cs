using System.ComponentModel.DataAnnotations.Schema;

namespace HappyHeadlines.Core.Entities;

public class Draft
{
    [Column("id")]
    public Guid Id { get; set; } = Guid.NewGuid();

    [Column("title")]
    public string Title { get; set; } = string.Empty;

    [Column("content")]
    public string Content { get; set; } = string.Empty;

    [Column("author")]
    public string Author { get; set; } = string.Empty;

    [Column("published_at")]
    public DateTime PublishedAt { get; set; }

    [Column("continent")]
    public Continent Continent { get; set; }
}