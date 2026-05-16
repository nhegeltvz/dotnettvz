using Data.Data;
using Data.Model.Interfaces;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using static Data.Data.Enums;

namespace Data.Model
{
    public class Ticket : ITicket
    {
        public Guid Id { get; set; }
        [Required]
        [MaxLength(50)]
        public string Title { get; set; } = string.Empty;

        [MaxLength(500)]
        public string Description { get; set; } = string.Empty;

        public TicketStatus Status { get; set; }
        [Required]
        [Range(1,5)]
        public int Priority { get; set; }

        public DateTime CreatedAt { get; set; }

        public int CategoryId { get; set; }

        public string AssigneeId { get; set; }

        public string ReporterId { get; set; }

        //Ef core navigation properties

        [ForeignKey(nameof(CategoryId))]
        public virtual Category Category { get; set; } = null!;
        [ForeignKey(nameof(AssigneeId))]
        [InverseProperty(nameof(User.AssignedTickets))]
        public virtual User Assignee { get; set; } = null!;
        [ForeignKey(nameof(ReporterId))]
        [InverseProperty(nameof(User.ReviewingTickets))]
        public virtual User Reporter { get; set; } = null!;
        public virtual ICollection<Comment> Comments { get; set; } = [];
        public virtual ICollection<AuditLog> AuditLogs { get; set; } = [];


    }
}
