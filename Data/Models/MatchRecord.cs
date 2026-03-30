using Data.Models.Interfaces;

namespace Data.Models;

public class MatchRecord : IMatchRecord
{
    public Guid Id { get; set; }

    public bool WasMatchHeld { get; set; }
    public DateTime MatchHeld { get; set; }
    public Guid PlayingFieldId { get; set; }
    public int GoalsTeamA { get; set; }
    public int GoalsTeamB { get; set; }

    //EF navigation properties
    public PlayingField PlayingField { get; set; } = null!;
    public List<MatchPlayer> MatchPlayers { get; set; } = new();
    public List<MatchVote> MatchVotes { get; set; } = new();
}
