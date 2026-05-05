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
    public virtual PlayingField PlayingField { get; set; } = null!;
    public virtual ICollection<MatchPlayer> MatchPlayers { get; set; } = new List<MatchPlayer>();
    public virtual ICollection<MatchVote> MatchVotes { get; set; } = new List<MatchVote>();
}
