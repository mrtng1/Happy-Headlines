using System.ComponentModel.DataAnnotations;

namespace HappyHeadlines.WebApp.Client.Models;

public class PublishArticleRequest
{
    [Required, StringLength(160)]
    public string Title { get; set; } = string.Empty;

    [Required, MinLength(10)]
    public string Content { get; set; } = string.Empty;
}
