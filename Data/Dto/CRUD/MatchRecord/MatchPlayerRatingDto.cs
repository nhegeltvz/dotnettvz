using System.ComponentModel.DataAnnotations;

namespace Data.Dto.CRUD.MatchRecord;

public class MatchPlayerRatingDto
{
    public Guid PlayerGivingRatingId { get; set; }
    public Guid PlayerReceivingRatingId { get; set; }

    [Range(1, 5, ErrorMessage = "Ocjena mora biti između 1 i 5.")]
    public int Rating { get; set; }
}
