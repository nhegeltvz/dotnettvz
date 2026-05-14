using System.ComponentModel.DataAnnotations;

namespace Data.Dto.CRUD.ScheduledMatch;

public class ScheduledMatchFormDto
{
    public Guid? Id { get; set; }

    [Required]
    public Guid PlayingFieldId { get; set; }

    [Required]
    public Guid PartyId { get; set; }

    [Required]
    public DateTime MatchDate { get; set; }
}
