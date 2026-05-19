using Data.Data;
using Data.Data.Common;
using Data.Models;
using Data.Services.Validation.Interfaces;

namespace Data.Services.Validation
{
    public class MatchPlayerValidator : IValidator<MatchPlayer>
    {
        private readonly MatchTrackerDbContext _dbContext;

        public MatchPlayerValidator(MatchTrackerDbContext dbContext)
        {
            _dbContext = dbContext;
        }

        public Result Validate(MatchPlayer entity)
        {
            var errors = new List<Error>();

            if (entity.PlayerId == Guid.Empty)
            {
                errors.Add(new Error(ErrorType.Validation, "Player is required."));
            }
            else if (!_dbContext.Players.Any(player => player.Id == entity.PlayerId))
            {
                errors.Add(new Error(ErrorType.Validation, "Player does not exist."));
            }

            if (entity.MatchRecordId == Guid.Empty)
            {
                errors.Add(new Error(ErrorType.Validation, "Match record is required."));
            }
            else if (!_dbContext.MatchRecords.Any(record => record.Id == entity.MatchRecordId))
            {
                errors.Add(new Error(ErrorType.Validation, "Match record does not exist."));
            }

            if (!Enum.IsDefined(typeof(Team), entity.Team))
            {
                errors.Add(new Error(ErrorType.Validation, "Team selection is invalid."));
            }

            if (entity.Goals < 0 || entity.Assists < 0)
            {
                errors.Add(new Error(ErrorType.Validation, "Goals and assists cannot be negative."));
            }

            if (entity.PlayerId != Guid.Empty && entity.MatchRecordId != Guid.Empty)
            {
                var duplicate = _dbContext.MatchPlayers.Any(matchPlayer =>
                    matchPlayer.Id != entity.Id
                    && matchPlayer.PlayerId == entity.PlayerId
                    && matchPlayer.MatchRecordId == entity.MatchRecordId);
                if (duplicate)
                {
                    errors.Add(new Error(ErrorType.Validation, "Player is already assigned to this match."));
                }
            }

            return errors.Count == 0 ? Result.Success() : Result.Failure(errors);
        }
    }
}
