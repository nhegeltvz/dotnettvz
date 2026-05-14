using System.ComponentModel.DataAnnotations;

namespace Data.Dto.CRUD.PlayerRating;

public class PlayerRatingFormDto
{
    public Guid? Id { get; set; }

    public Guid MatchPlayerId { get; set; }

    public Guid PlayerGivingRatingId { get; set; }

    public Guid PlayerReceivingRatingId { get; set; }

    [Required]
    [Range(1, 10)]
    public int Rating { get; set; }
}
