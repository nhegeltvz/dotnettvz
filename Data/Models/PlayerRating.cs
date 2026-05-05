using Data.Models.Interfaces;

namespace Data.Models;

public class PlayerRating : IPlayerRating
{
    public Guid Id { get; set; }
    public Guid MatchPlayerId { get; set; }
    public Guid PlayerGivingRatingId { get; set; }
    public Guid PlayerReceivingRatingId { get; set; }
    public int Rating { get; set; }

    //EF navigation properties
    public virtual MatchPlayer MatchPlayer { get; set; } = null!;
    public virtual Player PlayerGivingRating { get; set; } = null!;
    public virtual Player PlayerReceivingRating { get; set; } = null!;
}
