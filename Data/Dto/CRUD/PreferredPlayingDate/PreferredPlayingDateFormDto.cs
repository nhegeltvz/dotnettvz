using System.ComponentModel.DataAnnotations;

namespace Data.Dto.CRUD.PreferredPlayingDate;

public class PreferredPlayingDateFormDto
{
    public Guid? Id { get; set; }

    [Required(ErrorMessage = "Grupa je obavezna.")]
    public Guid PartyId { get; set; }

    [Required(ErrorMessage = "Datum je obavezan.")]
    public DateTime Date { get; set; }
}
