using Data.Data;
using System.ComponentModel.DataAnnotations;

namespace Data.Dto.CRUD.MatchPlayer;

public class MatchPlayerFormDto
{
    public Guid? Id { get; set; }

    [Required]
    public Guid PlayerId { get; set; }

    public Team Team { get; set; }

    [Required]
    public Guid MatchRecordId { get; set; }

    [Range(0, 999)]
    public int Goals { get; set; }

    [Range(0, 999)]
    public int Assists { get; set; }
}
