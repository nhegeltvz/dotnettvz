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
                errors.Add(new Error(ErrorType.Validation, "Igrač utakmice je obavezan."));
            }
            else if (!_dbContext.MatchPlayers.Any(matchPlayer => matchPlayer.Id == entity.MatchPlayerId))
            {
                errors.Add(new Error(ErrorType.Validation, "Igrač utakmice ne postoji."));
            }

            if (entity.PlayerGivingRatingId == Guid.Empty)
            {
                errors.Add(new Error(ErrorType.Validation, "Igrač koji daje ocjenu je obavezan."));
            }
            else if (!_dbContext.Players.Any(player => player.Id == entity.PlayerGivingRatingId))
            {
                errors.Add(new Error(ErrorType.Validation, "Igrač koji daje ocjenu ne postoji."));
            }

            if (entity.PlayerReceivingRatingId == Guid.Empty)
            {
                errors.Add(new Error(ErrorType.Validation, "Igrač koji prima ocjenu je obavezan."));
            }
            else if (!_dbContext.Players.Any(player => player.Id == entity.PlayerReceivingRatingId))
            {
                errors.Add(new Error(ErrorType.Validation, "Igrač koji prima ocjenu ne postoji."));
            }

            if (entity.PlayerGivingRatingId != Guid.Empty
                && entity.PlayerGivingRatingId == entity.PlayerReceivingRatingId)
            {
                errors.Add(new Error(ErrorType.Validation, "Igrač ne može ocjenjivati samog sebe."));
            }

            if (entity.Rating < 1 || entity.Rating > 10)
            {
                errors.Add(new Error(ErrorType.Validation, "Ocjena mora biti između 1 i 10."));
            }

            if (entity.MatchPlayerId != Guid.Empty)
            {
                var duplicate = _dbContext.PlayerRatings.Any(rating =>
                    rating.Id != entity.Id
                    && rating.MatchPlayerId == entity.MatchPlayerId);
                if (duplicate)
                {
                    errors.Add(new Error(ErrorType.Validation, "Ocjena za ovog igrača već postoji."));
                }
            }

            return errors.Count == 0 ? Result.Success() : Result.Failure(errors);
        }
    }
}
