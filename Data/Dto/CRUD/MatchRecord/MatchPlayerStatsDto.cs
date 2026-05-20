namespace Data.Dto.CRUD.MatchRecord;

public class MatchPlayerStatsDto
{
    public Guid PlayerId { get; set; }
    public int Goals { get; set; }
    public int Assists { get; set; }
}
