using System.ComponentModel.DataAnnotations;

namespace Data.Dto.CRUD.ScheduledMatch;

public class ScheduledMatchFormDto
{
    public Guid? Id { get; set; }

    [Required(ErrorMessage = "Teren je obavezan.")]
    public Guid PlayingFieldId { get; set; }

    [Required(ErrorMessage = "Grupa je obavezna.")]
    public Guid PartyId { get; set; }

    [Required(ErrorMessage = "Datum meča je obavezan.")]
    public DateTime MatchDate { get; set; }
}
