using Data.Data;
using Data.Models.Interfaces;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Data.Models;

public class MatchPlayer : IMatchPlayer
{
    [Key]
    public Guid Id { get; set; }
    [ForeignKey(nameof(Player))]
    public Guid PlayerId { get; set; }
    public Team Team { get; set; }
    [ForeignKey(nameof(MatchRecord))]
    public Guid MatchRecordId { get; set; }
    public int Goals { get; set; }
    public int Assists { get; set; }

    //EF navigation properties

    //Player is essentially a user, so Match player is connected to a matcha and a Player
    public virtual AppUser Player { get; set; } = null!;
    //If a match was played, it holds a match record, this player is connected to that match record
    public virtual MatchRecord MatchRecord { get; set; } = null!;
    //Player gives rating to other players in the match
    public virtual PlayerRating? PlayerRating { get; set; }
}
