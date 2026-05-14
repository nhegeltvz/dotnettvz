using Data.Models.Interfaces;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Data.Models
{
    public class ScheduledMatch : IScheduledMatch
    {
        [Key]
        public Guid Id { get; set; }
        [ForeignKey(nameof(PlayingField))]
        public Guid PlayingFieldId { get; set; }
        [ForeignKey(nameof(Party))]
        public Guid PartyId { get; set; }
        public DateTime MatchDate { get; set; }

        //EF navigation properties
        public virtual PlayingField PlayingField { get; set; } = null!;
        public virtual Party Party { get; set; } = null!;
        public virtual ICollection<ScheduledMatchAttendance> ScheduledMatchAttendances { get; set; } = new List<ScheduledMatchAttendance>();
    }
}
