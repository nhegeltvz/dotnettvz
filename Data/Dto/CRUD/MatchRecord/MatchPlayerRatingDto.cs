using System.ComponentModel.DataAnnotations;

namespace Data.Dto.CRUD.MatchRecord;

public class MatchPlayerRatingDto
{
    public Guid PlayerGivingRatingId { get; set; }
    public Guid PlayerReceivingRatingId { get; set; }

    [Range(1, 5)]
    public int Rating { get; set; }
}
