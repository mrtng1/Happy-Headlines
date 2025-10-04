namespace HappyHeadlines.ArticleService.Entities;

using System.ComponentModel.DataAnnotations.Schema;

public class Article
{
    [Column("id")]
    public Guid Id { get; set; } = Guid.NewGuid();

    [Column("title")]
    public string Title { get; set; } = "";

    [Column("content")]
    public string Content { get; set; } = "";

    [Column("author")]
    public string Author { get; set; } = "";

    [Column("published_at")]
    public DateTime PublishedAt { get; set; }

    [Column("continent")]
    public Continent Continent { get; set; }
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