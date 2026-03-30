namespace Data.Models.Interfaces;

public interface IPlayerRating
{
    Guid Id { get; }
    Guid MatchPlayerId { get; }
    Guid PlayerGivingRatingId { get; }
    Guid PlayerReceivingRatingId { get; }
    int Rating { get; }
}
