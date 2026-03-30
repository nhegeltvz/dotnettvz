namespace Data.Models.Interfaces;

public interface IPreferredPlayingDate
{
    Guid Id { get; }
    Guid PartyId { get; }
    DateTime Date { get; }
}
