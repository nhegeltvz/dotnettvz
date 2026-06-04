using Data.Models.Interfaces;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Data.Models;

public class Party : IParty
{
    [Key]
    public Guid Id { get; set; }
    [ForeignKey(nameof(PlayerCreated))]
    public Guid PlayerCreatedId { get; set; }
    public DateTime DateCreated { get; set; }
    public int MaxMembers { get; set; }
    public string PartyDescription { get; set; } = string.Empty;
    public string PreferredLocations { get; set; } = string.Empty;

    //EF navigation properties
    public virtual AppUser PlayerCreated { get; set; } = null!;
    public virtual ICollection<PreferredPlayingDate> PreferredPlayingDates { get; set; } = new List<PreferredPlayingDate>();
    public virtual ICollection<AppUser> Members { get; set; } = new List<AppUser>();
    public virtual ScheduledMatch? ScheduledMatch { get; set; }
}
