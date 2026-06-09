using Data.Data;
using Data.Data.Common;
using Data.Models;
using Data.Services.Validation.Interfaces;

namespace Data.Services.Validation
{
    public class PlayerRatingValidator : IValidator<PlayerRating>
    {
        private readonly MatchTrackerDbContext _dbContext;

        public PlayerRatingValidator(MatchTrackerDbContext dbContext)
        {
            _dbContext = dbContext;
        }

        public Result Validate(PlayerRating entity)
        {
            var errors = new List<Error>();

            if (entity.MatchPlayerId == Guid.Empty)
            {
                errors.Add(new Error(ErrorType.Validation, "Match player is required."));
            }
            else if (!_dbContext.MatchPlayers.Any(matchPlayer => matchPlayer.Id == entity.MatchPlayerId))
            {
                errors.Add(new Error(ErrorType.Validation, "Match player does not exist."));
            }

            if (entity.PlayerGivingRatingId == Guid.Empty)
            {
                errors.Add(new Error(ErrorType.Validation, "Player giving rating is required."));
            }
            else if (!_dbContext.Players.Any(player => player.Id == entity.PlayerGivingRatingId))
            {
                errors.Add(new Error(ErrorType.Validation, "Player giving rating does not exist."));
            }

            if (entity.PlayerReceivingRatingId == Guid.Empty)
            {
                errors.Add(new Error(ErrorType.Validation, "Player receiving rating is required."));
            }
            else if (!_dbContext.Players.Any(player => player.Id == entity.PlayerReceivingRatingId))
            {
                errors.Add(new Error(ErrorType.Validation, "Player receiving rating does not exist."));
            }

            if (entity.PlayerGivingRatingId != Guid.Empty
                && entity.PlayerGivingRatingId == entity.PlayerReceivingRatingId)
            {
                errors.Add(new Error(ErrorType.Validation, "Player cannot rate themselves."));
            }

            if (entity.Rating < 1 || entity.Rating > 10)
            {
                errors.Add(new Error(ErrorType.Validation, "Rating must be between 1 and 5."));
            }

            if (entity.MatchPlayerId != Guid.Empty)
            {
                var duplicate = _dbContext.PlayerRatings.Any(rating =>
                    rating.Id != entity.Id
                    && rating.MatchPlayerId == entity.MatchPlayerId);
                if (duplicate)
                {
                    errors.Add(new Error(ErrorType.Validation, "Rating already exists for this match player."));
                }
            }

            return errors.Count == 0 ? Result.Success() : Result.Failure(errors);
        }
    }
}
