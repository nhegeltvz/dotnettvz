using Data.Models.Interfaces;

namespace Data.Models;

public class PreferredPlayingDate : IPreferredPlayingDate
{
    public Guid Id { get; set; }
    public Guid PartyId { get; set; }
    public DateTime Date { get; set; }

    //EF navigation properties
    public Party Party { get; set; } = null!;
}
