using System.Net.Http.Json;
using System.Text.Json;
using HappyHeadlines.WebApp.Client.Models;

namespace HappyHeadlines.WebApp.Client.Services;

public class PublisherApi
{
    private readonly HttpClient _http;
    public PublisherApi(HttpClient http) => _http = http;

    public async Task<(bool ok, string message)> PublishAsync(PublishArticleRequest req, CancellationToken ct = default)
    {
        var resp = await _http.PostAsJsonAsync("/api/Publisher/publish", req, ct);
        var text = await resp.Content.ReadAsStringAsync(ct);

        if (resp.IsSuccessStatusCode)
        {
            // server returns: { "message": "Article published to queue successfully." }
            try
            {
                using var doc = JsonDocument.Parse(text);
                if (doc.RootElement.TryGetProperty("message", out var m))
                    return (true, m.GetString() ?? "Published.");
            }
            catch { /* ignore */ }
            return (true, "Published.");
        }

        // Surface ProblemDetails if present
        try
        {
            using var doc = JsonDocument.Parse(text);
            var title = doc.RootElement.TryGetProperty("title", out var t) ? t.GetString() : null;
            var detail = doc.RootElement.TryGetProperty("detail", out var d) ? d.GetString() : null;
            var msg = string.Join(" ", new[] { title, detail }.Where(s => !string.IsNullOrWhiteSpace(s)));
            return (false, string.IsNullOrWhiteSpace(msg) ? text : msg);
        }
        catch
        {
            return (false, string.IsNullOrWhiteSpace(text)
                ? $"HTTP {(int)resp.StatusCode} {resp.ReasonPhrase}"
                : text);
        }
    }
}
