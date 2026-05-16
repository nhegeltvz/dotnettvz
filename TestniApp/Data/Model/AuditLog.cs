using Data.Model.Interfaces;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using static Data.Data.Enums;

namespace Data.Model
{
    public class AuditLog : IAuditLog
    {
        [Key]
        public Guid Id { get; set; }
        public DateTime DateTicketStatusChanged { get; set; }

        public Guid TicketId { get; set; }
        public string ChangedByUserId { get; set; }
        public TicketStatus OldStatus { get; set; }
        public TicketStatus NewStatus { get; set; }

        //Ef core navigation properties

        [ForeignKey(nameof(TicketId))]
        public virtual Ticket Ticket { get; set; } = null!;
        [ForeignKey(nameof(ChangedByUserId))]
        public virtual User UserChanged { get; set; } = null!;

    }
}
