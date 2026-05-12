using Data.Models.Interfaces;

namespace Data.Models
{
    public class ScheduledMatchAttendance : IScheduledMatchAttendance
    {
        public Guid Id { get; set; }
        public Guid ScheduledMatchId { get; set; }
        public Guid PlayerId { get; set; }
        public bool IsAttending { get; set; }

        //EF navigation properties

        public virtual ScheduledMatch ScheduledMatch { get; set; } = null!;
        public virtual Player Player { get; set; } = null!;
    }
}
