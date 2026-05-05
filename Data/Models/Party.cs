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
    public virtual Player PlayerCreated { get; set; } = null!;
    public virtual ICollection<PreferredPlayingDate> PreferredPlayingDates { get; set; } = new List<PreferredPlayingDate>();
    public virtual ICollection<Player> Members { get; set; } = new List<Player>();
}
