using Microsoft.Extensions.Configuration;
using System.Net.Http.Json;
using System.Text.Json;
using System.Text.Json.Serialization;
using static System.Net.WebRequestMethods;

namespace Data.Services
{
    public class AiMatchParserService
    {
        private readonly HttpClient _http;
        private readonly string _apiKey;
        private readonly string _model;

        public AiMatchParserService(HttpClient httpClient, IConfiguration configuration)
        {
            _http = httpClient;
            _apiKey = configuration["Groq:ApiKey"]!;
            _model = configuration["Groq:Model"] ?? "llama-3.3-70b-versatile";
        }

        public async Task<MatchParseResult> ParseAsync(string userText, List<PlayingFieldOption> fields)
        {
            var fieldList = string.Join("\n", fields.Select(f => $"- {f.Id}: {f.Name} (lat: {f.Latitude}, lon: {f.Longitude})"));
            var today = DateTime.Now.ToString("yyyy-MM-dd HH:mm");

            var jsonExample = "{ \"matchDate\": \"yyyy-MM-ddTHH:mm:ss\", \"suggestedFields\": [ { \"id\": \"field-guid\", \"name\": \"field name\", \"confidence\": \"high|medium|low\" } ] }";

            var systemPrompt = $"You are a match scheduling assistant. Current date and time: {today}. " +
                $"The user will describe when and where they want to play in Croatian or English. " +
                $"Each field has coordinates (lat, lon). When the user mentions a neighborhood, street, or area, use the coordinates to find the geographically closest fields — do not guess by name alone. " +
                $"Available playing fields:\n{fieldList}\n" +
                $"Return ONLY valid JSON, no markdown, no explanation: {jsonExample} " +
                "Return up to 3 suggested fields ordered by proximity/relevance. If date is unclear return null for matchDate.";

            var requestBody = new
            {
                model = _model,
                messages = new[]
                {
                    new { role = "system", content = systemPrompt },
                    new { role = "user",   content = userText }
                },
                temperature = 0.2
            };

            var request = new HttpRequestMessage(HttpMethod.Post, "https://api.groq.com/openai/v1/chat/completions");
            request.Headers.Add("Authorization", $"Bearer {_apiKey}");
            request.Content = JsonContent.Create(requestBody);

            var response = await _http.SendAsync(request);

            if (!response.IsSuccessStatusCode)
            {
                var error = await response.Content.ReadAsStringAsync();
                throw new Exception($"Groq API error {response.StatusCode}: {error}");
            }

            var json = await response.Content.ReadFromJsonAsync<JsonElement>();

            var raw = json
                .GetProperty("choices")[0]
                .GetProperty("message")
                .GetProperty("content")
                .GetString()!;

            raw = raw.Trim();
            if (raw.StartsWith("```"))
            {
                raw = raw[(raw.IndexOf('\n') + 1)..];
                raw = raw[..raw.LastIndexOf("```")].Trim();
            }

            return JsonSerializer.Deserialize<MatchParseResult>(raw)!;
        }

    }
}


public class PlayingFieldOption
{
    public Guid Id { get; set; }
    public string Name { get; set; } = string.Empty;
    public double Longitude { get; set; }
    public double Latitude { get; set; }
}

public class MatchParseResult
{
    [JsonPropertyName("matchDate")]
    public DateTime? MatchDate { get; set; }

    [JsonPropertyName("suggestedFields")]
    public List<SuggestedField> SuggestedFields { get; set; } = new();
}

public class SuggestedField
{
    [JsonPropertyName("id")]
    public Guid Id { get; set; }

    [JsonPropertyName("name")]
    public string Name { get; set; } = string.Empty;

    [JsonPropertyName("confidence")]
    public string Confidence { get; set; } = string.Empty;
}