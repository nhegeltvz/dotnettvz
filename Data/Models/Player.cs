using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Data.Data;

namespace Data.Models;

public class Player
{
    [Key]
    public Guid Id { get; set; }

    [ForeignKey(nameof(User))]
    public Guid UserId { get; set; }

    public string Bio { get; set; } = string.Empty;
    public Position PreferredPosition { get; set; }
    public DateOnly DateOfBirth { get; set; }

    // Navigation back to Identity user (for display name: User.UserName)
    public virtual AppUser User { get; set; } = null!;

    // Domain navigation properties (moved from AppUser)
    public virtual ICollection<Party> CreatedParties { get; set; } = new List<Party>();
    public virtual ICollection<Party> JoinedParties { get; set; } = new List<Party>();
    public virtual ICollection<MatchPlayer> MatchPlayers { get; set; } = new List<MatchPlayer>();
    public virtual ICollection<PlayerRating> RatingsGiven { get; set; } = new List<PlayerRating>();
    public virtual ICollection<PlayerRating> RatingsReceived { get; set; } = new List<PlayerRating>();
    public virtual ICollection<MatchVote> MatchVotes { get; set; } = new List<MatchVote>();
    public virtual ICollection<ScheduledMatchAttendance> ScheduledMatchAttendances { get; set; } = new List<ScheduledMatchAttendance>();
}
