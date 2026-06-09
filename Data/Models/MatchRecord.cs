using Data.Models.Interfaces;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Data.Models;

public class MatchRecord : IMatchRecord
{
    [Key]
    public Guid Id { get; set; }

    public bool WasMatchHeld { get; set; }
    public DateTime MatchHeld { get; set; }
    [ForeignKey(nameof(PlayingField))]
    public Guid PlayingFieldId { get; set; }
    public int GoalsTeamA { get; set; }
    public int GoalsTeamB { get; set; }

    [ForeignKey(nameof(ScheduledMatch))]
    public Guid? ScheduledMatchId { get; set; }

    //EF navigation properties
    public virtual ScheduledMatch? ScheduledMatch { get; set; }
    public virtual PlayingField PlayingField { get; set; } = null!;
    public virtual ICollection<MatchPlayer> MatchPlayers { get; set; } = new List<MatchPlayer>();
    public virtual ICollection<MatchVote> MatchVotes { get; set; } = new List<MatchVote>();
}
