namespace Data.Models.Interfaces
{
    public interface IScheduledMatch
    {
        public Guid Id { get; }
        public Guid PlayingFieldId { get; }
        public Guid PartyId { get; }
        public DateTime MatchDate { get; }
    }
}
