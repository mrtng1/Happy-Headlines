using System.Net.Http.Json;
using HappyHeadlines.WebApp.Client.Models;

namespace HappyHeadlines.WebApp.Client.Services;

public class ArticleApi(HttpClient http)
{
    readonly HttpClient _http = http;

    public async Task<List<Article>> GetPageAsync(int page = 1, string continent = "Global", CancellationToken ct = default)
        => await _http.GetFromJsonAsync<List<Article>>($"/api/Articles/GetArticles?page={page}&continent={continent}", ct)
           ?? new();

    public async Task<Article?> GetAsync(Guid id, string continent = "Global", CancellationToken ct = default)
        => await _http.GetFromJsonAsync<Article>($"/api/Articles/{id}?continent={continent}", ct);
}
