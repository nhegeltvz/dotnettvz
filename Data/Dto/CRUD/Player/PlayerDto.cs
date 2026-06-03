using Data.Data;
using System.Linq.Expressions;
using PlayerModel = Data.Models.Player;

namespace Data.Dto.CRUD.Player
{
    public class PlayerDto
    {
        public Guid Id { get; set; }
        public string Username { get; set; } = string.Empty;
        public string Email { get; set; } = string.Empty;
        public string Bio { get; set; } = string.Empty;
        public Position PreferredPosition { get; set; }
        public int? Age { get; set; }
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
                Username = player.Username,
                Email = player.Email,
                Bio = player.Bio,
                PreferredPosition = player.PreferredPosition,
                Age = player.Age,
                CreatedPartiesCount = player.CreatedParties.Count,
                JoinedPartiesCount = player.JoinedParties.Count,
                MatchesPlayedCount = player.MatchPlayers.Where(matchPlayer => matchPlayer.MatchRecord.WasMatchHeld).Count(),
                RatingsGivenCount = player.RatingsGiven.Count,
                RatingsReceivedCount = player.RatingsReceived.Count,
                AverageMatchRating = player.RatingsReceived
                    .Select(rating => (double?)rating.Rating)
                    .Average() ?? 0.0
            };
        }
    }
}
