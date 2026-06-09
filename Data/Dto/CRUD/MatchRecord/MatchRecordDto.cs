using Data.Data;
using System.Linq.Expressions;
using MatchPlayerModel = Data.Models.MatchPlayer;
using MatchRecordModel = Data.Models.MatchRecord;
using MatchVoteModel = Data.Models.MatchVote;
using PlayingFieldModel = Data.Models.PlayingField;

namespace Data.Dto.CRUD.MatchRecord
{
    public class MatchRecordDto
    {
        public Guid Id { get; set; }

        public bool WasMatchHeld { get; set; }
        public DateTime MatchHeld { get; set; }
        public int GoalsTeamA { get; set; }
        public int GoalsTeamB { get; set; }
        public PlayingFieldModel PlayingField { get; set; } = null!;
        public List<MatchPlayerSummaryDto> MatchPlayers { get; set; } = new();
        public List<MatchVoteDto> MatchVotes { get; set; } = new();


        public static Expression<Func<MatchRecordModel, MatchRecordDto>> ToDto()
        {
            return matchRecord => new MatchRecordDto
            {
                Id = matchRecord.Id,
                WasMatchHeld = matchRecord.WasMatchHeld,
                MatchHeld = matchRecord.MatchHeld,
                GoalsTeamA = matchRecord.GoalsTeamA,        
                GoalsTeamB = matchRecord.GoalsTeamB,
                PlayingField = matchRecord.PlayingField,
                MatchPlayers = matchRecord.MatchPlayers == null
                    ? new List<MatchPlayerSummaryDto>()
                    : matchRecord.MatchPlayers
                        .Select(matchPlayer => new MatchPlayerSummaryDto
                        {
                            Id = matchPlayer.Id,
                            PlayerId = matchPlayer.PlayerId,
                            PlayerUsername = matchPlayer.Player == null ? string.Empty : matchPlayer.Player.User.UserName ?? string.Empty,
                            Team = matchPlayer.Team,
                            Goals = matchPlayer.Goals,
                            Assists = matchPlayer.Assists,
                            PlayerRating = matchPlayer.PlayerRating == null ? (int?)null : matchPlayer.PlayerRating.Rating,
                            PlayerRatingByUsername = matchPlayer.PlayerRating == null
                                ? string.Empty
                                : matchPlayer.PlayerRating.PlayerGivingRating == null
                                    ? string.Empty
                                    : matchPlayer.PlayerRating.PlayerGivingRating.User.UserName ?? string.Empty
                        })
                        .ToList(),
                MatchVotes = matchRecord.MatchVotes == null
                    ? new List<MatchVoteDto>()
                    : matchRecord.MatchVotes
                        .Select(matchVote => new MatchVoteDto
                        {
                            PlayerId = matchVote.PlayerId,
                            VotedHeld = matchVote.VotedHeld
                        })
                        .ToList(),
            };
        }
    }

    public class MatchPlayerSummaryDto
    {
        public Guid Id { get; set; }
        public Guid PlayerId { get; set; }
        public string PlayerUsername { get; set; } = string.Empty;
        public Team Team { get; set; }
        public int Goals { get; set; }
        public int Assists { get; set; }
        public int? PlayerRating { get; set; }
        public string PlayerRatingByUsername { get; set; } = string.Empty;
    }

    public class MatchVoteDto
    {
        public Guid PlayerId { get; set; }
        public bool VotedHeld { get; set; }
    }
}
