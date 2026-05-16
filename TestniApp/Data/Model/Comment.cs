using Data.Model.Interfaces;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Data.Model
{
    public class Comment : IComment
    {
        public Guid Id { get; set; }
        [MaxLength(500)]
        public string Text { get; set; } = string.Empty;
        public string AuthorId { get; set; }
        public DateTime CommentedAt { get; set; }
        public Guid TicketId { get; set; }

        //Ef core navigation properties

        [ForeignKey(nameof(AuthorId))]
        public virtual User Author { get; set; } = null!;
        [ForeignKey(nameof(TicketId))]
        public virtual Ticket Ticket { get; set; } = null!;
    }
}
