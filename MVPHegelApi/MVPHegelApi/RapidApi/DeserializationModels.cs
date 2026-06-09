using System.Text.Json.Serialization;

namespace MVPHegelApi.RapidApi
{
    public class DeserializationModels
    {
        public record StandingResponse(
    [property: JsonPropertyName("response")] StandingBody Response);
        public record StandingBody(
            [property: JsonPropertyName("standing")] List<StandingTeam> Standing);
        public record StandingTeam(
            [property: JsonPropertyName("id")] int Id,
            [property: JsonPropertyName("name")] string Name);

        public record MatchesResponse(
            [property: JsonPropertyName("response")] MatchesBody Response);
        public record MatchesBody(
            [property: JsonPropertyName("matches")] List<RawMatch> Matches);
        public record RawMatch(
            [property: JsonPropertyName("id")] string Id,
            [property: JsonPropertyName("home")] RawTeamScore Home,
            [property: JsonPropertyName("away")] RawTeamScore Away,
            [property: JsonPropertyName("status")] RawStatus Status);
        public record RawTeamScore(
            [property: JsonPropertyName("id")] string Id,
            [property: JsonPropertyName("name")] string Name,
            [property: JsonPropertyName("score")] int? Score);
        public record RawStatus(
            [property: JsonPropertyName("utcTime")] DateTime UtcTime,
            [property: JsonPropertyName("finished")] bool Finished);
    }
}
