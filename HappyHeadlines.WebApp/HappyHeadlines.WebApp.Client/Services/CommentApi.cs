// HappyHeadlines.WebApp.Client/Services/CommentApi.cs
using System.Net.Http.Json;
using HappyHeadlines.WebApp.Client.Models;

namespace HappyHeadlines.WebApp.Client.Services;

public class CommentApi(HttpClient http)
{
    readonly HttpClient _http = http;

    // RET: brug route /api/Comment/article/{id}
    public async Task<List<CommentItem>> GetForArticleAsync(Guid articleId, CancellationToken ct = default)
        => await _http.GetFromJsonAsync<List<CommentItem>>($"/api/Comment/article/{articleId}", ct) ?? new();

    // RET: post til /api/Comment/CreateComment
    public async Task<(bool ok, string? error)> CreateAsync(CreateCommentRequest req, CancellationToken ct = default)
    {
        var resp = await _http.PostAsJsonAsync("/api/Comment/CreateComment", req, ct);
        return resp.IsSuccessStatusCode
            ? (true, null)
            : (false, await resp.Content.ReadAsStringAsync(ct));
    }
}
