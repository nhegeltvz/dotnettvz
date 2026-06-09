using Data.Models.Interfaces;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Data.Models;

//After date of match ends, each player is asked wether or not the match was played.
public class MatchVote : IMatchVote
{
    [Key]
    public Guid Id { get; set; }

    [ForeignKey(nameof(MatchRecord))]
    public Guid MatchRecordId { get; set; }

    [ForeignKey(nameof(Player))]
    public Guid PlayerId { get; set; }
    public bool VotedHeld { get; set; }

    //EF navigation properties
    public virtual MatchRecord MatchRecord { get; set; } = null!;
    public virtual Player Player { get; set; } = null!;
}
