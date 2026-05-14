using System.ComponentModel.DataAnnotations;

namespace Data.Dto.CRUD.PreferredPlayingDate;

public class PreferredPlayingDateFormDto
{
    public Guid? Id { get; set; }

    [Required]
    public Guid PartyId { get; set; }

    [Required]
    public DateTime Date { get; set; }
}
