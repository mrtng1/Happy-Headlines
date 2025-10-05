namespace HappyHeadlines.WebApp.Client.Models;

public class Article
{
    public Guid Id { get; set; }
    public string Title { get; set; } = "";
    public string Content { get; set; } = "";
    public string Author { get; set; } = "";
    public Continent Continent { get; set; }   // ← enum i stedet for string
    public DateTime PublishedAt { get; set; }
}
