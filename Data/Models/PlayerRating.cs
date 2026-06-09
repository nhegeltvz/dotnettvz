using Data.Models.Interfaces;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Data.Models;

public class PlayerRating : IPlayerRating
{
    [Key]
    public Guid Id { get; set; }
    [ForeignKey(nameof(MatchPlayer))]
    public Guid MatchPlayerId { get; set; }
    [ForeignKey(nameof(PlayerGivingRating))]
    public Guid PlayerGivingRatingId { get; set; }
    [ForeignKey(nameof(PlayerReceivingRating))]
    public Guid PlayerReceivingRatingId { get; set; }
    public int Rating { get; set; }

    //EF navigation properties
    public virtual MatchPlayer MatchPlayer { get; set; } = null!;
    public virtual Player PlayerGivingRating { get; set; } = null!;
    public virtual Player PlayerReceivingRating { get; set; } = null!;
}
