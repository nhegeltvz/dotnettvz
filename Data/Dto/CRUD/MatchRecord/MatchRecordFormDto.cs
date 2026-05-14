using System.ComponentModel.DataAnnotations;

namespace Data.Dto.CRUD.MatchRecord;

public class MatchRecordFormDto
{
    public Guid? Id { get; set; }

    [Required]
    public bool WasMatchHeld { get; set; }

    [Required]
    public DateTime MatchHeld { get; set; }

    public Guid PlayingFieldId { get; set; }

    [Required]
    [Range(0, 100)]
    public int GoalsTeamA { get; set; }

    [Required]
    [Range(0, 100)]
    public int GoalsTeamB { get; set; }
}
