using Data.Models.Interfaces;

namespace Data.Models;

public class Party : IParty
{
    public Guid Id { get; set; }
    public Guid PlayerCreatedId { get; set; }
    public DateTime DateCreated { get; set; }
    public int MaxMembers { get; set; }
    public string PartyDescription { get; set; } = string.Empty;
    public string PreferredLocations { get; set; } = string.Empty;

    //EF navigation properties
    public Player PlayerCreated { get; set; } = null!;
    public List<PreferredPlayingDate> PreferredPlayingDates { get; set; } = new();
    public List<Player> Members { get; set; } = new();
}
