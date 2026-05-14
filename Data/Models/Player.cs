using Data.Data;
using Data.Models.Interfaces;
using System.ComponentModel.DataAnnotations;

namespace Data.Models;

public class Player : IPlayer
{
    [Key]
    public Guid Id { get; set; }
    public string Username { get; set; } = string.Empty;
    public string Email { get; set; } = string.Empty;
    public string Bio { get; set; } = string.Empty;
    public Position PreferredPosition { get; set; }
    public int? Age { get; set; }
    public byte[] ProfilePicture { get; set; } = [];

    //EF navigation properties
    public virtual ICollection<Party> CreatedParties { get; set; } = new List<Party>();
    public virtual ICollection<Party> JoinedParties { get; set; } = new List<Party>();
    public virtual ICollection<MatchPlayer> MatchPlayers { get; set; } = new List<MatchPlayer>();
    public virtual ICollection<PlayerRating> RatingsGiven { get; set; } = new List<PlayerRating>();
    public virtual ICollection<PlayerRating> RatingsReceived { get; set; } = new List<PlayerRating>();
    public virtual ICollection<MatchVote> MatchVotes { get; set; } = new List<MatchVote>();
    public virtual ICollection<ScheduledMatchAttendance> ScheduledMatchAttendances { get; set; } = new List<ScheduledMatchAttendance>();
}
