

using MVPHegelApi;
using static MVPHegelApi.RapidApi.DeserializationModels;

public class HnlService
{
    private readonly IHttpClientFactory _factory;
    private readonly string _baseImageUrl;
    private List<Team>? _teamsCache;
    private List<Match>? _matchesCache;

    public HnlService(IHttpClientFactory factory, IConfiguration config) {
        _baseImageUrl = config["ApiSettings:BaseImageUrl"]!;
        _factory = factory;
    } 

    public async Task<List<Team>> GetTeamsAsync()
    {
        if (_teamsCache != null) return _teamsCache;

        var client = _factory.CreateClient("RapidApi");
        var response = await client.GetFromJsonAsync<StandingResponse>(
            "/football-get-standing-all?leagueid=252");

        _teamsCache = response!.Response.Standing.Select(t => new Team
        {
            Id = t.Id.ToString(),
            Name = t.Name,
            LogoUrl = $"{_baseImageUrl}/images/{GetLogoFileName(t.Id)}",
            WebUrl = GetWebUrl(t.Id)
        }).ToList();

        return _teamsCache;
    }

    public async Task<List<Match>> GetMatchesAsync()
    {
        if (_matchesCache != null) return _matchesCache;

        var client = _factory.CreateClient("RapidApi");
        var response = await client.GetFromJsonAsync<MatchesResponse>(
            "/football-get-all-matches-by-league?leagueid=252");

        _matchesCache = response!.Response.Matches.Select(m => new Match
        {
            Id = m.Id,
            HomeTeam = m.Home.Name,
            HomeTeamId = m.Home.Id,
            AwayTeam = m.Away.Name,
            AwayTeamId = m.Away.Id,
            HomeScore = m.Status.Finished ? m.Home.Score.ToString() : null,
            AwayScore = m.Status.Finished ? m.Away.Score.ToString() : null,
            Date = m.Status.UtcTime,
            Finished = m.Status.Finished
        }).ToList();

        return _matchesCache;
    }

    private static string GetLogoFileName(int teamId) => teamId switch
    {
        10156 => "dinamo_zagreb.png",
        10154 => "hajduk_split.png",
        10162 => "rijeka.png",
        10157 => "osijek.png",
        6038 => "istra.png",
        175388 => "lokomotiva.png",
        10165 => "varazdin.svg",
        206560 => "gorica.png",
        1581 => "slaven_belupo.png",
        45228 => "vukovar.png",
        _ => "default.png"
    };

    private static string? GetWebUrl(int teamId) => teamId switch
    {
        10156 => "https://gnkdinamo.hr/hr",
        10154 => "https://hajduk.hr/",
        10162 => "https://nk-rijeka.hr/",
        10157 => "https://nk-osijek.hr/",
        6038 => "https://www.facebook.com/NKIstra1961Pula/",
        175388 => "https://nklokomotiva.hr/",
        10165 => "https://varazdin.club/",
        206560 => "https://hnk-gorica.hr/",
        1581 => "https://nk-slaven-belupo.hr/",
        45228 => "https://hnk-vukovar1991.hr/",
        _ => null
    };



}