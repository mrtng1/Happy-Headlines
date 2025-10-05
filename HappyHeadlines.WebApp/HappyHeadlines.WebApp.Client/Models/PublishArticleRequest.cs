using System.ComponentModel.DataAnnotations;

namespace HappyHeadlines.WebApp.Client.Models;

public class PublishArticleRequest
{
    [Required, StringLength(160)]
    public string Title { get; set; } = string.Empty;

    [Required, MinLength(20)]
    public string Content { get; set; } = string.Empty;

    [Required, StringLength(80)]
    public string Author { get; set; } = string.Empty;

    // Hold den som string, så vi ikke skal dele enum mellem apps
    [Required]
    public string Continent { get; set; } = "Global";
}
