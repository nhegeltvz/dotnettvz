using System.Linq.Expressions;
using PlayerModel = Data.Models.Player;

namespace Data.Dto.CRUD.Player
{
    public class PlayerDto
    {
        public Guid Id { get; set; }
        public Guid UserId { get; set; }
        public string Username { get; set; } = string.Empty;   // from Player.User.UserName
        public string Bio { get; set; } = string.Empty;
        public string PreferredPosition { get; set; } = string.Empty;
        public DateOnly DateOfBirth { get; set; }
        public int? CreatedPartiesCount { get; set; }
        public int? JoinedPartiesCount { get; set; }
        public int? MatchesPlayedCount { get; set; }
        public int? RatingsGivenCount { get; set; }
        public int? RatingsReceivedCount { get; set; }
        public double AverageMatchRating { get; set; }

        public static Expression<Func<PlayerModel, PlayerDto>> ToDto()
        {
            return player => new PlayerDto
            {
                Id = player.Id,
                UserId = player.UserId,
                Username = player.User.UserName ?? string.Empty,
                Bio = player.Bio,
                PreferredPosition = player.PreferredPosition.ToString(),
                DateOfBirth = player.DateOfBirth,
                CreatedPartiesCount = player.CreatedParties.Count,
                JoinedPartiesCount = player.JoinedParties.Count,
                MatchesPlayedCount = player.MatchPlayers
                    .Where(mp => mp.MatchRecord.WasMatchHeld)
                    .Count(),
                RatingsGivenCount = player.RatingsGiven.Count,
                RatingsReceivedCount = player.RatingsReceived.Count,
                AverageMatchRating = player.RatingsReceived
                    .Select(r => (double?)r.Rating)
                    .Average() ?? 0.0
            };
        }
    }
}
