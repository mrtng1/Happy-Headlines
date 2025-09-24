using System.ComponentModel.DataAnnotations.Schema;

namespace HappyHeadlines.Core.Entities;

public class Draft
{
    [Column("id")]
    public Guid Id { get; set; } = Guid.NewGuid();

    [Column("author_id")]
    public Guid AuthorId { get; set; }

    [Column("title")]
    public string Title { get; set; } = string.Empty;

    [Column("content")]
    public string Content { get; set; } = string.Empty;

    [Column("continent")]
    public Continent Continent { get; set; }
    
    [Column("created_at")]
    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;

    [Column("last_modified_at")]
    public DateTime LastModifiedAt { get; set; } = DateTime.UtcNow;

    [Column("version")]
    public int Version { get; set; } = 1;
}

public enum Continent
{
    Global = 0,
    Africa = 1,
    Antarctica = 2,
    Asia = 3,
    Europe = 4,
    NorthAmerica = 5,
    Australia = 6,
    SouthAmerica = 7
}