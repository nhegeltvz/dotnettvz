using Data.Models.Interfaces;

namespace Data.Models
{
    public class ScheduledMatch : IScheduledMatch
    {
        public Guid Id { get; set; }
        public Guid PlayingFieldId { get; set; }
        public Guid PartyId { get; set; }
        public DateTime MatchDate { get; set; }

        //EF navigation properties
        public virtual PlayingField PlayingField { get; set; } = null!;
        public virtual Party Party { get; set; } = null!;
        public virtual ICollection<ScheduledMatchAttendance> ScheduledMatchAttendances { get; set; } = new List<ScheduledMatchAttendance>();
    }
}
