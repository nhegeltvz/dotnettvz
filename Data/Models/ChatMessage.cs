using Data.Models.Interfaces;

namespace Data.Models
{
    public class ChatMessage : IChatMessage
    {
        public int Id { get; set; }
        public Guid PartyId { get; set; }
        public Guid SenderUserId { get; set; }
        public string SenderUsername { get; set; } = string.Empty;
        public string Text { get; set; } = string.Empty;
        public DateTime SentAt { get; set; } = DateTime.UtcNow;

        //Ef Core Navigation Properites
        public virtual Party Party { get; set; } = null!;
        public virtual AppUser Sender { get; set; } = null!;

    }
}
