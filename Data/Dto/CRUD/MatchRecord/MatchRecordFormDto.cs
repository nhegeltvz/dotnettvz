using System.ComponentModel.DataAnnotations;
namespace Data.Dto.CRUD.MatchRecord;

public class MatchRecordFormDto
{
    public Guid? Id { get; set; }

    [Required(ErrorMessage = "Navedite je li meč odigran.")]
    public bool WasMatchHeld { get; set; }

    [Required(ErrorMessage = "Datum odigravanja je obavezan.")]
    public DateTime MatchHeld { get; set; }

    public Guid PlayingFieldId { get; set; }

    [Required(ErrorMessage = "Broj golova ekipe A je obavezan.")]
    [Range(0, 100, ErrorMessage = "Broj golova mora biti između 0 i 100.")]
    public int GoalsTeamA { get; set; }

    [Required(ErrorMessage = "Broj golova ekipe B je obavezan.")]
    [Range(0, 100, ErrorMessage = "Broj golova mora biti između 0 i 100.")]
    public int GoalsTeamB { get; set; }

    public List<Guid> MatchPlayerIds { get; set; } = [];

    public List<MatchPlayerRatingDto> PlayerRatings { get; set; } = [];

    public List<MatchPlayerStatsDto> MatchPlayerStats { get; set; } = [];

    public override string ToString() =>
        $"Date={MatchHeld:yyyy-MM-dd}, Field={PlayingFieldId}, Score={GoalsTeamA}-{GoalsTeamB}, Held={WasMatchHeld}, Players={MatchPlayerIds.Count}";
}
