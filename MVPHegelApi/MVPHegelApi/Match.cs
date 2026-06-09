namespace MVPHegelApi
{
    public class Match
    {
        public string Id { get; set; }
        public string HomeTeam { get; set; }
        public string HomeTeamId { get; set; }
        public string AwayTeam { get; set; }
        public string AwayTeamId { get; set; }
        public string? HomeScore { get; set; }
        public string? AwayScore { get; set; }
        public DateTime Date { get; set; }
        public bool Finished { get; set; }
    }
}
