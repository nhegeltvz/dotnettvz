using Data.Data;
using Data.Models.Interfaces;

namespace Data.Models;

public class Player : IPlayer
{
    public Guid Id { get; set; }
    public string Username { get; set; } = string.Empty;
    public string Email { get; set; } = string.Empty;
    public string Bio { get; set; } = string.Empty;
    public Position PreferredPosition { get; set; }
    public int? Age { get; set; }
    public byte[] ProfilePicture { get; set; } = [];

    //EF navigation properties
    public List<Party> CreatedParties { get; set; } = new();
    public List<Party> JoinedParties { get; set; } = new();
    public List<MatchPlayer> MatchPlayers { get; set; } = new();
    public List<PlayerRating> RatingsGiven { get; set; } = new();
    public List<PlayerRating> RatingsReceived { get; set; } = new();
    public List<MatchVote> MatchVotes { get; set; } = new();
}
