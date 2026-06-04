using Data.Data;
using Data.Models.Interfaces;
using Microsoft.AspNetCore.Identity;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Data.Models
{
    public class AppUser : IdentityUser<Guid>, IPlayer
    {
        [Required]
        [StringLength(11, MinimumLength = 11)]
        [RegularExpression("^[0-9]*$")]
        public string OIB { get; set; } = string.Empty;

        [Required]
        [StringLength(13, MinimumLength = 13)]
        [RegularExpression("^[0-9]*$")]
        public string JMBG { get; set; } = string.Empty;

        // Domain fields from Player
        [Column("PlayerUsername")]
        public string Username { get; set; } = string.Empty;
        public string Bio { get; set; } = string.Empty;
        public Position PreferredPosition { get; set; }
        public int? Age { get; set; }
        public byte[] ProfilePicture { get; set; } = [];

        // IPlayer: Id and Email are satisfied by IdentityUser<Guid>
        Guid IPlayer.Id => Id;
        string IPlayer.Email => Email ?? string.Empty;
        byte[] IPlayer.ProfilePicture { set => ProfilePicture = value; }

        // Navigation properties from Player
        public virtual ICollection<Party> CreatedParties { get; set; } = new List<Party>();
        public virtual ICollection<Party> JoinedParties { get; set; } = new List<Party>();
        public virtual ICollection<MatchPlayer> MatchPlayers { get; set; } = new List<MatchPlayer>();
        public virtual ICollection<PlayerRating> RatingsGiven { get; set; } = new List<PlayerRating>();
        public virtual ICollection<PlayerRating> RatingsReceived { get; set; } = new List<PlayerRating>();
        public virtual ICollection<MatchVote> MatchVotes { get; set; } = new List<MatchVote>();
        public virtual ICollection<ScheduledMatchAttendance> ScheduledMatchAttendances { get; set; } = new List<ScheduledMatchAttendance>();
    }
}
