using System.ComponentModel.DataAnnotations;

namespace Data.Dto.CRUD.PlayerRating;

public class PlayerRatingFormDto
{
    public Guid? Id { get; set; }

    public Guid MatchPlayerId { get; set; }

    public Guid PlayerGivingRatingId { get; set; }

    public Guid PlayerReceivingRatingId { get; set; }

    [Required(ErrorMessage = "Ocjena je obavezna.")]
    [Range(1, 10, ErrorMessage = "Ocjena mora biti između 1 i 10.")]
    public int Rating { get; set; }
}
