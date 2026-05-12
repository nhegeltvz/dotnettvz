namespace Data.Models.Interfaces
{
    public interface IScheduledMatchAttendance
    {
        Guid Id { get; }
        Guid ScheduledMatchId { get; }
        Guid PlayerId { get; }
        bool IsAttending { get; }
    }
}
