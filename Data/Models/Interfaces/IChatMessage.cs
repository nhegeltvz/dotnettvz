namespace Data.Models.Interfaces
{
    public interface IChatMessage
    {
        public int Id { get; }
        public Guid PartyId { get; }
        public Guid SenderUserId { get; }
        public string SenderUsername { get; }
        public string Text { get; }
        public DateTime SentAt { get; }
    }
}
