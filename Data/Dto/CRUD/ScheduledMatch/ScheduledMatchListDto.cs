namespace Data.Dto.CRUD.ScheduledMatch;

public class ScheduledMatchListDto
{
    public Guid Id { get; set; }
    public Guid PlayingFieldId { get; set; }
    public string PlayingFieldName { get; set; } = string.Empty;
    public Guid PartyId { get; set; }
    public string PartyDescription { get; set; } = string.Empty;
    public DateTime MatchDate { get; set; }
}
