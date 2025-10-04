using HappyHeadlines.CommentService.Interfaces;

namespace HappyHeadlines.CommentService.Services;

using System.Text;
using System.Text.Json;
using Polly.CircuitBreaker;

public class ProfanityClient : IProfanityClient
{
    private readonly HttpClient _httpClient;
    private readonly ILogger<ProfanityClient> _logger;

    public ProfanityClient(HttpClient httpClient, ILogger<ProfanityClient> logger)
    {
        _httpClient = httpClient;
        _logger = logger;
    }

    public async Task<bool> ContainsProfanityAsync(string text)
    {
        try
        {
            var content = new StringContent($"\"{text}\"", Encoding.UTF8, "application/json");
            HttpResponseMessage response = await _httpClient.PostAsync("api/Profanity/check", content);

            response.EnsureSuccessStatusCode();

            var result = await response.Content.ReadAsStringAsync();
            return JsonSerializer.Deserialize<bool>(result, new JsonSerializerOptions { PropertyNameCaseInsensitive = true });
        }
        catch (BrokenCircuitException)
        {
            _logger.LogWarning("Circuit breaker is open. ProfanityService is unavailable.");
            throw;
        }
        catch (HttpRequestException ex)
        {
            _logger.LogError(ex, "An HTTP error occurred while calling ProfanityService.");
            throw;
        }
    }
}