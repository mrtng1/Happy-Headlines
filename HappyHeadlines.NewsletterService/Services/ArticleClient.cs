namespace HappyHeadlines.NewsletterService.Services
{
    public class ArticleClient
    {
        private readonly HttpClient _http;
        private readonly ILogger<ArticleClient> _log;
        public ArticleClient(HttpClient http, ILogger<ArticleClient> log) => (_http, _log) = (http, log);

        public record ArticleDto(Guid Id, string Title, string Summary, string Url, DateTime PublishedAt, string Continent);

        public async Task<List<ArticleDto>> GetByRangeAsync(DateTime fromUtc, DateTime toUtc, string continent)
        {
            var url = $"api/articles/range?fromUtc={fromUtc:o}&toUtc={toUtc:o}&continent={continent}";
            var resp = await _http.GetAsync(url);
            resp.EnsureSuccessStatusCode();
            return (await resp.Content.ReadFromJsonAsync<List<ArticleDto>>())!;
        }
    }
}
