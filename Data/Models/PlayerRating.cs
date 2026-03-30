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
    public MatchPlayer MatchPlayer { get; set; } = null!;
    public Player PlayerGivingRating { get; set; } = null!;
    public Player PlayerReceivingRating { get; set; } = null!;
}
