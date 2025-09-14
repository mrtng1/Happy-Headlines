namespace HappyHeadlines.Core.Entities;

public class Article
{
    public Guid Id { get; set; } = Guid.NewGuid();
    public string Title { get; set; } = string.Empty;
    public string Content { get; set; } = string.Empty;
    public string Author { get; set; } = string.Empty;
    public DateTime PublishedAt { get; set; }
    public Continent Continent { get; set; }
}

public enum Continent
{
    Undefined = 0,
    Africa = 1,
    Antarctica = 2,
    Asia = 3,
    Europe = 4,
    NorthAmerica = 5,
    Australia = 6,
    SouthAmerica = 7
}