using Data.Data;
using System.ComponentModel.DataAnnotations;

namespace Data.Dto.CRUD.MatchPlayer;

public class MatchPlayerFormDto
{
    public Guid? Id { get; set; }

    [Required(ErrorMessage = "Igrač je obavezan.")]
    public Guid PlayerId { get; set; }

    public Team Team { get; set; }

    [Required(ErrorMessage = "Utakmica je obavezna.")]
    public Guid MatchRecordId { get; set; }

    [Range(0, 999, ErrorMessage = "Broj golova mora biti između 0 i 999.")]
    public int Goals { get; set; }

    [Range(0, 999, ErrorMessage = "Broj asistencija mora biti između 0 i 999.")]
    public int Assists { get; set; }
}
