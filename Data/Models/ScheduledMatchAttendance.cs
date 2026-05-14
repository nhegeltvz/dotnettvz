using Data.Models.Interfaces;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Data.Models
{
    public class ScheduledMatchAttendance : IScheduledMatchAttendance
    {
        [Key]
        public Guid Id { get; set; }
        [ForeignKey(nameof(ScheduledMatch))]
        public Guid ScheduledMatchId { get; set; }
        [ForeignKey(nameof(Player))]
        public Guid PlayerId { get; set; }
        public bool IsAttending { get; set; }

        //EF navigation properties

        public virtual ScheduledMatch ScheduledMatch { get; set; } = null!;
        public virtual Player Player { get; set; } = null!;
    }
}
