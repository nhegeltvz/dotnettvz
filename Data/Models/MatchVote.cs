using Data.Models.Interfaces;

namespace Data.Models;

//After date of match ends, each player is asked wether or not the match was played.
public class MatchVote : IMatchVote
{
    public Guid Id { get; set; }
    public Guid MatchRecordId { get; set; }
    public Guid PlayerId { get; set; }
    public bool VotedHeld { get; set; }

    //EF navigation properties
    public virtual MatchRecord MatchRecord { get; set; } = null!;
    public virtual Player Player { get; set; } = null!;
}
