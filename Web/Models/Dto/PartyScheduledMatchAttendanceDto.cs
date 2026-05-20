namespace Web.Models.Dto;

public class PartyScheduledMatchAttendanceDto
{
    public Guid? Id { get; set; }
    public Guid PlayerId { get; set; }
    public bool IsAttending { get; set; }
    public string PlayerName { get; set; } = string.Empty;
}
